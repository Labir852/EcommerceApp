using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using ECommerceApp.Shared.Models;

namespace ECommerceApp.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public ProductSearchResultDto? Products { get; set; }
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

    public async Task OnGetAsync(string? searchTerm = null, string? sortBy = null, bool sortDescending = false, int page = 1)
    {
        SearchTerm = searchTerm;
        SortBy = sortBy;
        SortDescending = sortDescending;
        CurrentPage = page;

        var client = _clientFactory.CreateClient("API");
        var response = await client.GetAsync($"api/products?searchTerm={searchTerm}&sortBy={sortBy}&sortDescending={sortDescending}&page={page}&pageSize={PageSize}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Products = JsonSerializer.Deserialize<ProductSearchResultDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId)
    {
        var client = _clientFactory.CreateClient("API");
        var userId = "demo-user"; // In a real app, get this from authentication

        var response = await client.PostAsJsonAsync(
            $"api/cart/{userId}/items",
            new { ProductId = productId, Quantity = 1 });

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Item added to cart successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to add item to cart.";
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
