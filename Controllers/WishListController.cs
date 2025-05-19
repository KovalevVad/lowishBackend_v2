using lowishBackend_v2.DTOs;
using lowishBackend_v2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lowishBackend_v2.Controllers
{
    [Route("api/users/{userId}/wishlists")]
    [ApiController]
    public class WishListsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WishListsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users/5/wishlists
        [HttpGet]
        public async Task<IActionResult> GetUserWishlists(int userId)
        {
            // Проверяем, существует ли пользователь
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound($"Пользователь с ID {userId} не найден");
            }

            // Получаем вишлисты пользователя и преобразуем в DTO
            var wishlists = await _context.WishLists
                .Where(w => w.UserId == userId)
                .Select(w => new WishListResponse
                {
                    Id = w.Id,
                    Title = w.Title,
                    Description = w.Description,
                    UserId = w.UserId,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();

            return Ok(wishlists);
        }

        // GET: api/users/5/wishlists/3
        [HttpGet("{wishlistId}")]
        public async Task<IActionResult> GetWishlist(int userId, int wishlistId)
        {
            // Ищем вишлист для конкретного пользователя
            var wishlist = await _context.WishLists
                .Where(w => w.Id == wishlistId && w.UserId == userId)
                .Select(w => new WishListResponse
                {
                    Id = w.Id,
                    Title = w.Title,
                    Description = w.Description,
                    UserId = w.UserId,
                    CreatedAt = w.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (wishlist == null)
            {
                return NotFound($"Вишлист с ID {wishlistId} для пользователя {userId} не найден");
            }

            return Ok(wishlist);
        }

        // POST: api/users/5/wishlists
        [HttpPost]
        public async Task<IActionResult> CreateWishlist(int userId, WishListRequest request)
        {
            // Проверяем, существует ли пользователь
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound($"Пользователь с ID {userId} не найден");
            }

            // Создаем новый вишлист на основе DTO
            var wishlist = new WishList
            {
                Title = request.Title,
                Description = request.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            // Добавляем вишлист в базу данных
            _context.WishLists.Add(wishlist);
            await _context.SaveChangesAsync();

            // Преобразуем вишлист в DTO для ответа
            var response = new WishListResponse
            {
                Id = wishlist.Id,
                Title = wishlist.Title,
                Description = wishlist.Description,
                UserId = wishlist.UserId,
                CreatedAt = wishlist.CreatedAt
            };

            return CreatedAtAction(
                nameof(GetWishlist),
                new { userId = userId, wishlistId = wishlist.Id },
                response);
        }

        // PUT: api/users/5/wishlists/3
        [HttpPut("{wishlistId}")]
        public async Task<IActionResult> UpdateWishlist(int userId, int wishlistId, WishListRequest request)
        {
            // Проверяем, существует ли вишлист для этого пользователя
            var existingWishlist = await _context.WishLists
                .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId);

            if (existingWishlist == null)
            {
                return NotFound($"Вишлист с ID {wishlistId} для пользователя {userId} не найден");
            }

            // Обновляем только нужные поля из DTO
            existingWishlist.Title = request.Title;
            existingWishlist.Description = request.Description;

            // Сохраняем изменения в базе данных
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WishlistExists(wishlistId))
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

        // DELETE: api/users/5/wishlists/3
        [HttpDelete("{wishlistId}")]
        public async Task<IActionResult> DeleteWishlist(int userId, int wishlistId)
        {
            // Ищем вишлист для указанного пользователя
            var wishlist = await _context.WishLists
                .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId);

            if (wishlist == null)
            {
                return NotFound($"Вишлист с ID {wishlistId} для пользователя {userId} не найден");
            }

            // Удаляем вишлист
            _context.WishLists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WishlistExists(int id)
        {
            return _context.WishLists.Any(w => w.Id == id);
        }
    }
}