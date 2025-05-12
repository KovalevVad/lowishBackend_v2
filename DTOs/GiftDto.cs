using System.ComponentModel.DataAnnotations;

namespace lowishBackend_v2.DTOs
{
    // DTO для создания подарка
    public class CreateGiftRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        [Url]
        public string? ProductUrl { get; set; }
    }

    // DTO для обновления подарка
    public class UpdateGiftRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        [Url]
        public string? ProductUrl { get; set; }
    }

    // DTO для отображения подарка
    public class GiftResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ProductUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int WishlistId { get; set; }
    }
}