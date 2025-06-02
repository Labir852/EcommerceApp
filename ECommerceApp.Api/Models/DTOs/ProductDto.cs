using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Api.Models.DTOs;

public class ProductDto
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
}

public class ProductSearchDto
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

public class ProductListDto
{
    public List<ProductDto> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
} 