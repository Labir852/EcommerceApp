using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Api.Data;
using ECommerceApp.Api.Models;
using ECommerceApp.Api.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace ECommerceApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ProductListDto>> GetProducts([FromQuery] ProductSearchDto searchDto)
    {
        _logger.LogInformation("Getting products with search parameters: {@SearchDto}", searchDto);

        var query = _context.Products.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(searchDto.SearchTerm) || 
                p.Description.Contains(searchDto.SearchTerm));
        }

        // Apply sorting
        query = searchDto.SortBy?.ToLower() switch
        {
            "name" => searchDto.SortDescending 
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "price" => searchDto.SortDescending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        try
        {
            // Calculate pagination
            var totalItems = await query.CountAsync();
            _logger.LogInformation("Total items found: {TotalItems}", totalItems);

            var totalPages = (int)Math.Ceiling(totalItems / (double)searchDto.PageSize);
            var items = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountedPrice = p.DiscountedPrice,
                    DiscountStartDate = p.DiscountStartDate,
                    DiscountEndDate = p.DiscountEndDate,
                    DiscountPercentage = p.DiscountPercentage,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {ItemCount} items for page {Page}", items.Count, searchDto.Page);

            // Update discounted prices based on current date
            var currentDate = DateTime.UtcNow.Date;
            foreach (var product in items)
            {
                if (product.DiscountStartDate <= currentDate && 
                    product.DiscountEndDate >= currentDate)
                {
                    product.DiscountedPrice = product.Price * (1 - product.DiscountPercentage / 100);
                }
                else
                {
                    product.DiscountedPrice = null;
                }
            }

            var result = new ProductListDto
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = searchDto.Page,
                TotalPages = totalPages
            };

            _logger.LogInformation("Returning product list with {@Result}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            DiscountedPrice = product.DiscountedPrice,
            DiscountStartDate = product.DiscountStartDate,
            DiscountEndDate = product.DiscountEndDate,
            DiscountPercentage = product.DiscountPercentage,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl
        };

        // Update discounted price based on current date
        var currentDate = DateTime.UtcNow.Date;
        if (product.DiscountStartDate <= currentDate && 
            product.DiscountEndDate >= currentDate)
        {
            productDto.DiscountedPrice = product.Price * (1 - product.DiscountPercentage / 100);
        }
        else
        {
            productDto.DiscountedPrice = null;
        }

        return productDto;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            DiscountStartDate = productDto.DiscountStartDate,
            DiscountEndDate = productDto.DiscountEndDate,
            DiscountPercentage = productDto.DiscountPercentage,
            StockQuantity = productDto.StockQuantity,
            ImageUrl = productDto.ImageUrl
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        productDto.Id = product.Id;
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto)
    {
        if (id != productDto.Id)
        {
            return BadRequest();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.Price = productDto.Price;
        product.DiscountStartDate = productDto.DiscountStartDate;
        product.DiscountEndDate = productDto.DiscountEndDate;
        product.DiscountPercentage = productDto.DiscountPercentage;
        product.StockQuantity = productDto.StockQuantity;
        product.ImageUrl = productDto.ImageUrl;
        product.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
} 