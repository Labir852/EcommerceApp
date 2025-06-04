using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using ECommerceApp.Shared.Models;

namespace ECommerceApp.Web.Pages;

public class ProductListResponse
{
    public List<Product> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public List<Product> Products { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 9;

    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("/api/products");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response: {Content}", content);
                
                var productList = JsonSerializer.Deserialize<ProductListResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (productList != null)
                {
                    Products = productList.Items;
                    _logger.LogInformation("Deserialized {Count} products", Products.Count);
                }
                else
                {
                    _logger.LogWarning("Deserialized product list is null");
                }
            }
            else
            {
                _logger.LogError("Failed to fetch products. Status code: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
        }
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId)
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.PostAsync($"/api/cart/items?productId={productId}&quantity=1", null);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to add item to cart. Status code: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
        }

        return RedirectToPage();
    }

    public string GetPageUrl(int pageNumber)
    {
        return Url.Page("./Index", new
        {
            searchTerm = SearchTerm,
            sortBy = SortBy,
            sortDescending = SortDescending,
            page = pageNumber
        }) ?? "";
    }
}
