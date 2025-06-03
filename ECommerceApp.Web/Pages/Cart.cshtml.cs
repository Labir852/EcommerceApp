using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using ECommerceApp.Shared.Models;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ECommerceApp.Web.Pages;

public class CartModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<CartModel> _logger;

    public List<CartItem> CartItems { get; set; } = new();
    public decimal SubTotal => CartItems.Sum(item => item.Product.Price * item.Quantity);
    public decimal Total => SubTotal; // Add tax, shipping, etc. if needed

    public CartModel(IHttpClientFactory clientFactory, ILogger<CartModel> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("/api/cart");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                CartItems = JsonSerializer.Deserialize<List<CartItem>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<CartItem>();
            }
            else
            {
                _logger.LogError("Failed to fetch cart items. Status code: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cart items");
        }
    }

    public async Task<IActionResult> OnPostRemoveFromCartAsync(int productId)
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"/api/cart/items/{productId}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to remove item from cart. Status code: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart");
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckoutAsync()
    {
        try
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.PostAsync("/api/orders", null);
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/OrderConfirmation");
            }
            
            _logger.LogError("Failed to create order. Status code: {StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
        }

        return RedirectToPage();
    }
} 