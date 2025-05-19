using System.ComponentModel.DataAnnotations;

namespace lowishBackend_v2.DTOs
{
    // DTO для ответа с данными подарка
    public class GiftResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public decimal? Price { get; set; }

        public string? PurchaseUrl { get; set; }

        public string? Status { get; set; }

        public int WishListId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}