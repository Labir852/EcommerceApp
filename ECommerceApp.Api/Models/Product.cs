using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Api.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    public decimal? DiscountedPrice { get; set; }

    public DateTime? DiscountStartDate { get; set; }

    public DateTime? DiscountEndDate { get; set; }

    [Range(0, 100)]
    public decimal DiscountPercentage { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
} 