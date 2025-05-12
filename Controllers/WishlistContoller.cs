using lowishBackend_v2.DTOs;
using lowishBackend_v2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace lowishBackend_v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        // Получение всех вишлистов конкретного пользователя
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<WishlistResponse>>> GetUserWishlists(int userId)
        {
            // Проверяем, существует ли пользователь
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            // Получаем все вишлисты пользователя
            var wishlists = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => new WishlistResponse
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    CreatedAt = w.CreatedAt,
                    UserId = w.UserId,
                    Username = w.User.Username
                })
                .ToListAsync();

            return wishlists;
        }

        // Получение конкретного вишлиста по ID с подарками
        [HttpGet("{id}")]
        public async Task<ActionResult<WishlistDetailResponse>> GetWishlist(int id)
        {
            // Получаем вишлист с подарками
            var wishlist = await _context.Wishlists
                .Include(w => w.Gifts)
                .Include(w => w.User)
                .Where(w => w.Id == id)
                .Select(w => new WishlistDetailResponse
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    CreatedAt = w.CreatedAt,
                    UserId = w.UserId,
                    Username = w.User.Username,
                    Gifts = w.Gifts.Select(g => new GiftResponse
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description,
                        Price = g.Price,
                        ProductUrl = g.ProductUrl,
                        CreatedAt = g.CreatedAt,
                        WishlistId = g.WishlistId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (wishlist == null)
            {
                return NotFound();
            }

            return wishlist;
        }

        // Метод создания вишлиста с явным указанием ID пользователя
        [HttpPost("user/{userId}")]
        public async Task<ActionResult<WishlistResponse>> CreateWishlist(int userId, CreateWishlistRequest request)
        {
            // Проверяем, существует ли пользователь с указанным ID
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            // Создаем новый вишлист с явно указанным userId
            var wishlist = new Wishlist
            {
                Name = request.Name,
                Description = request.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            // Формируем ответ
            var response = new WishlistResponse
            {
                Id = wishlist.Id,
                Name = wishlist.Name,
                Description = wishlist.Description,
                CreatedAt = wishlist.CreatedAt,
                UserId = wishlist.UserId,
                Username = user.Username
            };

            return CreatedAtAction(nameof(GetWishlist), new { id = wishlist.Id }, response);
        }

        // Метод обновления вишлиста с явным указанием ID пользователя
        [HttpPut("{id}/user/{userId}")]
        public async Task<IActionResult> UpdateWishlist(int id, int userId, UpdateWishlistRequest request)
        {
            // Находим вишлист
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            // Проверяем, что вишлист принадлежит указанному пользователю
            if (wishlist.UserId != userId)
            {
                return BadRequest("Вишлист не принадлежит указанному пользователю");
            }

            // Обновляем данные вишлиста
            wishlist.Name = request.Name;
            wishlist.Description = request.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishlistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Метод удаления вишлиста с явным указанием ID пользователя
        [HttpDelete("{id}/user/{userId}")]
        public async Task<IActionResult> DeleteWishlist(int id, int userId)
        {
            // Находим вишлист
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            // Проверяем, что вишлист принадлежит указанному пользователю
            if (wishlist.UserId != userId)
            {
                return BadRequest("Вишлист не принадлежит указанному пользователю");
            }

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Вспомогательный метод для получения ID текущего пользователя из токена
        private int GetCurrentUserId()
        {
            // Сначала ищем claim "sub" (который используется в JwtService)
            var userIdClaim = User.FindFirst("sub");

            // Если не нашли, пробуем стандартный ClaimTypes.NameIdentifier (запасной вариант)
            if (userIdClaim == null)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            }

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Токен не содержит идентификатора пользователя");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Неверный формат идентификатора пользователя");
            }

            return userId;
        }

        private bool WishlistExists(int id)
        {
            return _context.Wishlists.Any(e => e.Id == id);
        }
    }
}