using System.ComponentModel.DataAnnotations;

namespace lowishBackend_v2.DTOs
{
    // DTO для создания вишлиста
    public class CreateWishlistRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }

    // DTO для обновления вишлиста
    public class UpdateWishlistRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }

    // DTO для отображения вишлиста
    public class WishlistResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
    }

    // DTO для подробного отображения вишлиста с подарками
    public class WishlistDetailResponse : WishlistResponse
    {
        public List<GiftResponse> Gifts { get; set; } = new List<GiftResponse>();
    }
}