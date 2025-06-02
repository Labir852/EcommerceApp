using System.Collections.Generic;

namespace ECommerceApp.Shared.Models;

public class CartDto
{
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
    public decimal UnitPrice { get; set; }
    public decimal? DiscountedUnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? DiscountedTotalPrice { get; set; }
}

public class UpdateCartItemDto
{
    public int Quantity { get; set; }
} 