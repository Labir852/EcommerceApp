using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Api.Models;

public class CartItem
{
    public int Id { get; set; }

    [Required]
    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? DiscountedUnitPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
} 