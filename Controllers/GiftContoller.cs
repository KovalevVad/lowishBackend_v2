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
    public class GiftController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GiftController(AppDbContext context)
        {
            _context = context;
        }

        // Получение всех подарков в конкретном вишлисте
        [HttpGet("wishlist/{wishlistId}")]
        public async Task<ActionResult<IEnumerable<GiftResponse>>> GetGiftsInWishlist(int wishlistId)
        {
            // Проверяем, существует ли вишлист
            var wishlist = await _context.Wishlists.FindAsync(wishlistId);
            if (wishlist == null)
            {
                return NotFound("Вишлист не найден");
            }

            // Получаем все подарки из вишлиста
            var gifts = await _context.Gifts
                .Where(g => g.WishlistId == wishlistId)
                .Select(g => new GiftResponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Price = g.Price,
                    ProductUrl = g.ProductUrl,
                    CreatedAt = g.CreatedAt,
                    WishlistId = g.WishlistId
                })
                .ToListAsync();

            return gifts;
        }

        // Получение конкретного подарка по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<GiftResponse>> GetGift(int id)
        {
            var gift = await _context.Gifts
                .Where(g => g.Id == id)
                .Select(g => new GiftResponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Price = g.Price,
                    ProductUrl = g.ProductUrl,
                    CreatedAt = g.CreatedAt,
                    WishlistId = g.WishlistId
                })
                .FirstOrDefaultAsync();

            if (gift == null)
            {
                return NotFound();
            }

            return gift;
        }

        // Добавление нового подарка в вишлист с явным указанием ID пользователя
        [HttpPost("wishlist/{wishlistId}/user/{userId}")]
        public async Task<ActionResult<GiftResponse>> AddGiftToWishlist(int wishlistId, int userId, CreateGiftRequest request)
        {
            // Проверяем, существует ли вишлист
            var wishlist = await _context.Wishlists.FindAsync(wishlistId);
            if (wishlist == null)
            {
                return NotFound("Вишлист не найден");
            }

            // Проверяем, что вишлист принадлежит указанному пользователю
            if (wishlist.UserId != userId)
            {
                return BadRequest("Вишлист не принадлежит указанному пользователю");
            }

            // Создаем новый подарок
            var gift = new Gift
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ProductUrl = request.ProductUrl,
                WishlistId = wishlistId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();

            // Формируем ответ
            var response = new GiftResponse
            {
                Id = gift.Id,
                Name = gift.Name,
                Description = gift.Description,
                Price = gift.Price,
                ProductUrl = gift.ProductUrl,
                CreatedAt = gift.CreatedAt,
                WishlistId = gift.WishlistId
            };

            return CreatedAtAction(nameof(GetGift), new { id = gift.Id }, response);
        }

        // Обновление подарка с явным указанием ID пользователя
        [HttpPut("{id}/user/{userId}")]
        public async Task<IActionResult> UpdateGift(int id, int userId, UpdateGiftRequest request)
        {
            // Находим подарок
            var gift = await _context.Gifts
                .Include(g => g.Wishlist)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gift == null)
            {
                return NotFound();
            }

            // Проверяем, что вишлист принадлежит указанному пользователю
            if (gift.Wishlist.UserId != userId)
            {
                return BadRequest("Подарок не принадлежит вишлисту указанного пользователя");
            }

            // Обновляем данные подарка
            gift.Name = request.Name;
            gift.Description = request.Description;
            gift.Price = request.Price;
            gift.ProductUrl = request.ProductUrl;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GiftExists(id))
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

        // Удаление подарка с явным указанием ID пользователя
        [HttpDelete("{id}/user/{userId}")]
        public async Task<IActionResult> DeleteGift(int id, int userId)
        {
            // Находим подарок
            var gift = await _context.Gifts
                .Include(g => g.Wishlist)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gift == null)
            {
                return NotFound();
            }

            // Проверяем, что вишлист принадлежит указанному пользователю
            if (gift.Wishlist.UserId != userId)
            {
                return BadRequest("Подарок не принадлежит вишлисту указанного пользователя");
            }

            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GiftExists(int id)
        {
            return _context.Gifts.Any(e => e.Id == id);
        }
    }
}