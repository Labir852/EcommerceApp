using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using ECommerceApp.Api.Models.DTOs;

namespace ECommerceApp.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public ProductListDto Products { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int CurrentPage { get; set; } = 1;
    public const int PageSize = 9;

    public IndexModel(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task OnGetAsync(string? searchTerm, string? sortBy, bool sortDescending = false, int page = 1)
    {
        SearchTerm = searchTerm;
        SortBy = sortBy;
        SortDescending = sortDescending;
        CurrentPage = page;

        var client = _clientFactory.CreateClient("API");
        var response = await client.GetAsync(
            $"api/products?searchTerm={searchTerm}&sortBy={sortBy}&sortDescending={sortDescending}&page={page}&pageSize={PageSize}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Products = JsonSerializer.Deserialize<ProductListDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new ProductListDto();
        }
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
    {
        var client = _clientFactory.CreateClient("API");
        var userId = "demo-user"; // In a real app, get this from authentication

        var response = await client.PostAsJsonAsync(
            $"api/cart/{userId}/items",
            new AddToCartDto { ProductId = productId, Quantity = quantity });

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
