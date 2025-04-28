using lowishBackend_v2.DTOs;
using lowishBackend_v2.Models;
using lowishBackend_v2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace lowishBackend_v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            // Проверяем, существует ли пользователь с таким email
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Email уже зарегистрирован"
                });
            }

            // Проверяем, существует ли пользователь с таким username
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Имя пользователя уже занято"
                });
            }

            // Хешируем пароль
            string passwordHash = BC.HashPassword(model.Password);

            // Создаем нового пользователя
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash,
                Name = model.Name
            };

            // Сохраняем пользователя в БД
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Генерируем JWT-токен
            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Success = true,
                Token = token,
                Username = user.Username,
                Message = "Регистрация прошла успешно"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            // Находим пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            // Если пользователь не найден или пароль неверный
            if (user == null || !BC.Verify(model.Password, user.PasswordHash))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Неверный email или пароль"
                });
            }

            // Генерируем JWT-токен
            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponse
            {
                Success = true,
                Token = token,
                Username = user.Username,
                Message = "Авторизация прошла успешно"
            });
        }
    }
}
