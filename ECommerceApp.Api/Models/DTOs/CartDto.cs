using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Api.Models.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public decimal? DiscountedTotalPrice { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? DiscountedUnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountedTotalPrice { get; set; }
}

public class AddToCartDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class UpdateCartItemDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
} 