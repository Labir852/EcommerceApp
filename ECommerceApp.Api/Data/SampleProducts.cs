using ECommerceApp.Api.Models;

namespace ECommerceApp.Api.Data;

public static class SampleProducts
{
    public static Product[] GetSampleProducts()
    {
        var now = DateTime.UtcNow;
        return new[]
        {
            new Product
            {
                Name = "Professional DSLR Camera",
                Description = "High-end digital camera with 45MP sensor",
                Price = 2499.99m,
                StockQuantity = 8,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 5,
                DiscountStartDate = now,
                DiscountEndDate = now.AddDays(10),
                CreatedAt = now
            },
            new Product
            {
                Name = "Ultra-Wide Gaming Monitor",
                Description = "49-inch curved gaming monitor, 5120x1440",
                Price = 1299.99m,
                StockQuantity = 12,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = now
            },
            new Product
            {
                Name = "Mechanical Keyboard",
                Description = "RGB mechanical keyboard with Cherry MX switches",
                Price = 159.99m,
                StockQuantity = 30,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 15,
                DiscountStartDate = now,
                DiscountEndDate = now.AddDays(5),
                CreatedAt = now
            },
            new Product
            {
                Name = "Wireless Gaming Mouse",
                Description = "Ultra-lightweight wireless gaming mouse",
                Price = 79.99m,
                StockQuantity = 45,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = now
            },
            new Product
            {
                Name = "4K Webcam",
                Description = "Professional 4K webcam with built-in microphone",
                Price = 199.99m,
                StockQuantity = 18,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 20,
                DiscountStartDate = now,
                DiscountEndDate = now.AddDays(7),
                CreatedAt = now
            },
            new Product
            {
                Name = "Ergonomic Office Chair",
                Description = "Fully adjustable ergonomic office chair",
                Price = 399.99m,
                StockQuantity = 15,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = now
            },
            new Product
            {
                Name = "Standing Desk",
                Description = "Electric height-adjustable standing desk",
                Price = 599.99m,
                StockQuantity = 10,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 10,
                DiscountStartDate = now,
                DiscountEndDate = now.AddDays(14),
                CreatedAt = now
            },
            new Product
            {
                Name = "Wireless Earbuds",
                Description = "True wireless earbuds with noise cancellation",
                Price = 249.99m,
                StockQuantity = 25,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = now
            },
            new Product
            {
                Name = "USB-C Dock",
                Description = "12-in-1 USB-C docking station",
                Price = 129.99m,
                StockQuantity = 20,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 25,
                DiscountStartDate = now,
                DiscountEndDate = now.AddDays(3),
                CreatedAt = now
            },
            new Product
            {
                Name = "Wireless Charging Pad",
                Description = "15W fast wireless charging pad",
                Price = 39.99m,
                StockQuantity = 50,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = now
            },
            new Product
            {
                Name = "Smart Speaker",
                Description = "Voice-controlled smart speaker with premium sound",
                Price = 199.99m,
                StockQuantity = 22,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 15,
                DiscountStartDate = now,
                DiscountEndDate = now.AddDays(7),
                CreatedAt = now
            },
            new Product
            {
                Name = "Portable SSD",
                Description = "1TB portable SSD with USB-C",
                Price = 159.99m,
                StockQuantity = 35,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = now
            },
            new Product
            {
                Name = "Gaming Console",
                Description = "Next-gen gaming console with 4K support",
                Price = 499.99m,
                StockQuantity = 8,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 10,
                DiscountStartDate = now,
                DiscountEndDate = now.AddDays(5),
                CreatedAt = now
            },
            new Product
            {
                Name = "Bluetooth Speaker",
                Description = "Waterproof portable Bluetooth speaker",
                Price = 89.99m,
                StockQuantity = 40,
                ImageUrl = "https://picsum.photos/800/600",
                CreatedAt = now
            },
            new Product
            {
                Name = "Smart Watch",
                Description = "Fitness tracking smartwatch with heart rate monitor",
                Price = 299.99m,
                StockQuantity = 15,
                ImageUrl = "https://picsum.photos/800/600",
                DiscountPercentage = 20,
                DiscountStartDate = now,
                DiscountEndDate = now.AddDays(10),
                CreatedAt = now
            }
        };
    }
} 