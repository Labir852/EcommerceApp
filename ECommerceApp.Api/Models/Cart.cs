using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Api.Models;

public class Cart
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
} 