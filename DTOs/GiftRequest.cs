using System.ComponentModel.DataAnnotations;

namespace lowishBackend_v2.DTOs
{
    public class GiftRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public decimal? Price { get; set; }

        public string? PurchaseUrl { get; set; }

        public string? Status { get; set; }
    }
}
