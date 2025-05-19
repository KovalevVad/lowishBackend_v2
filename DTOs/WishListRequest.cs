using System.ComponentModel.DataAnnotations;

namespace lowishBackend_v2.DTOs
{
    public class WishListRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
    }
}