namespace ECommerceApp.Shared.Models;

public class CartItem
{
    public int Id { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
} 