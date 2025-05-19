using lowishBackend_v2.DTOs;
using lowishBackend_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lowishBackend_v2.Controllers
{
    [Route("api/users/{userId}/wishlists/{wishlistId}/gifts")]
    [ApiController]
    public class GiftsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GiftsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users/5/wishlists/3/gifts
        [HttpGet]
        public async Task<IActionResult> GetGifts(int userId, int wishlistId)
        {
            // Проверяем, существует ли вишлист для указанного пользователя
            var wishlistExists = await _context.WishLists
                .AnyAsync(w => w.Id == wishlistId && w.UserId == userId);

            if (!wishlistExists)
            {
                return NotFound($"Вишлист с ID {wishlistId} для пользователя {userId} не найден");
            }

            // Получаем подарки и преобразуем в DTO
            var gifts = await _context.Gifts
                .Where(g => g.WishListId == wishlistId)
                .Select(g => new GiftResponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    Price = g.Price,
                    PurchaseUrl = g.PurchaseUrl,
                    Status = g.Status,
                    WishListId = g.WishListId,
                    CreatedAt = g.CreatedAt
                })
                .ToListAsync();

            return Ok(gifts);
        }

        // GET: api/users/5/wishlists/3/gifts/7
        [HttpGet("{giftId}")]
        public async Task<IActionResult> GetGift(int userId, int wishlistId, int giftId)
        {
            // Проверяем, существует ли вишлист для указанного пользователя
            var wishlistExists = await _context.WishLists
                .AnyAsync(w => w.Id == wishlistId && w.UserId == userId);

            if (!wishlistExists)
            {
                return NotFound($"Вишлист с ID {wishlistId} для пользователя {userId} не найден");
            }

            // Ищем подарок в указанном вишлисте
            var gift = await _context.Gifts
                .Where(g => g.Id == giftId && g.WishListId == wishlistId)
                .Select(g => new GiftResponse
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    Price = g.Price,
                    PurchaseUrl = g.PurchaseUrl,
                    Status = g.Status,
                    WishListId = g.WishListId,
                    CreatedAt = g.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (gift == null)
            {
                return NotFound($"Подарок с ID {giftId} в вишлисте {wishlistId} не найден");
            }

            return Ok(gift);
        }

        // POST: api/users/5/wishlists/3/gifts
        [HttpPost]
        public async Task<IActionResult> CreateGift(int userId, int wishlistId, GiftRequest request)
        {
            // Проверяем, существует ли вишлист для указанного пользователя
            var wishlist = await _context.WishLists
                .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId);

            if (wishlist == null)
            {
                return NotFound($"Вишлист с ID {wishlistId} для пользователя {userId} не найден");
            }

            // Создаем новый подарок на основе DTO
            var gift = new Gift
            {
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                Price = request.Price,
                PurchaseUrl = request.PurchaseUrl,
                Status = request.Status,
                WishListId = wishlistId,
                CreatedAt = DateTime.UtcNow
            };

            // Добавляем подарок в базу данных
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();

            // Преобразуем подарок в DTO для ответа
            var response = new GiftResponse
            {
                Id = gift.Id,
                Name = gift.Name,
                Description = gift.Description,
                ImageUrl = gift.ImageUrl,
                Price = gift.Price,
                PurchaseUrl = gift.PurchaseUrl,
                Status = gift.Status,
                WishListId = gift.WishListId,
                CreatedAt = gift.CreatedAt
            };

            return CreatedAtAction(
                nameof(GetGift),
                new { userId = userId, wishlistId = wishlistId, giftId = gift.Id },
                response);
        }

        // PUT: api/users/5/wishlists/3/gifts/7
        [HttpPut("{giftId}")]
        public async Task<IActionResult> UpdateGift(int userId, int wishlistId, int giftId, GiftRequest request)
        {
            // Проверяем, существует ли вишлист для указанного пользователя
            var wishlistExists = await _context.WishLists
                .AnyAsync(w => w.Id == wishlistId && w.UserId == userId);

            if (!wishlistExists)
            {
                return NotFound($"Вишлист с ID {wishlistId} для пользователя {userId} не найден");
            }

            // Ищем подарок в указанном вишлисте
            var gift = await _context.Gifts
                .FirstOrDefaultAsync(g => g.Id == giftId && g.WishListId == wishlistId);

            if (gift == null)
            {
                return NotFound($"Подарок с ID {giftId} в вишлисте {wishlistId} не найден");
            }

            // Обновляем поля подарка
            gift.Name = request.Name;
            gift.Description = request.Description;
            gift.ImageUrl = request.ImageUrl;
            gift.Price = request.Price;
            gift.PurchaseUrl = request.PurchaseUrl;
            gift.Status = request.Status;

            // Сохраняем изменения
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GiftExists(giftId))
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

        // DELETE: api/users/5/wishlists/3/gifts/7
        [HttpDelete("{giftId}")]
        public async Task<IActionResult> DeleteGift(int userId, int wishlistId, int giftId)
        {
            // Проверяем, существует ли вишлист для указанного пользователя
            var wishlistExists = await _context.WishLists
                .AnyAsync(w => w.Id == wishlistId && w.UserId == userId);

            if (!wishlistExists)
            {
                return NotFound($"Вишлист с ID {wishlistId} для пользователя {userId} не найден");
            }

            // Ищем подарок в указанном вишлисте
            var gift = await _context.Gifts
                .FirstOrDefaultAsync(g => g.Id == giftId && g.WishListId == wishlistId);

            if (gift == null)
            {
                return NotFound($"Подарок с ID {giftId} в вишлисте {wishlistId} не найден");
            }

            // Удаляем подарок
            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GiftExists(int id)
        {
            return _context.Gifts.Any(g => g.Id == id);
        }
    }
}