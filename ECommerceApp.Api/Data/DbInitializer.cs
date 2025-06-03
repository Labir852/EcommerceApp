using ECommerceApp.Api.Models;

namespace ECommerceApp.Api.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Check if there are any products
        if (context.Products.Any())
        {
            return; // DB has been seeded
        }

        var products = new[]
        {
            new Product
            {
                Name = "Gaming Laptop",
                Description = "High-performance gaming laptop with RTX 4080",
                Price = 1999.99m,
                StockQuantity = 10,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 10,
                DiscountStartDate = DateTime.UtcNow,
                DiscountEndDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Smartphone",
                Description = "Latest model with 5G capability",
                Price = 899.99m,
                StockQuantity = 15,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Wireless Headphones",
                Description = "Noise-cancelling Bluetooth headphones",
                Price = 299.99m,
                StockQuantity = 20,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 15,
                DiscountStartDate = DateTime.UtcNow,
                DiscountEndDate = DateTime.UtcNow.AddDays(14),
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "4K Smart TV",
                Description = "65-inch OLED Smart TV",
                Price = 1499.99m,
                StockQuantity = 5,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Coffee Maker",
                Description = "Programmable coffee maker with built-in grinder",
                Price = 199.99m,
                StockQuantity = 25,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 20,
                DiscountStartDate = DateTime.UtcNow,
                DiscountEndDate = DateTime.UtcNow.AddDays(3),
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
} 