using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using ECommerceApp.Shared.Models;

namespace ECommerceApp.Web.Pages;

public class CartModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public CartDto? Cart { get; set; }

    public CartModel(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task OnGetAsync()
    {
        var client = _clientFactory.CreateClient("API");
        var userId = "demo-user"; // In a real app, get this from authentication

        var response = await client.GetAsync($"api/cart/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Cart = JsonSerializer.Deserialize<CartDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

    public async Task<IActionResult> OnPostUpdateQuantityAsync(int itemId, int quantity)
    {
        var client = _clientFactory.CreateClient("API");
        var userId = "demo-user"; // In a real app, get this from authentication

        var response = await client.PutAsJsonAsync(
            $"api/cart/{userId}/items/{itemId}",
            new UpdateCartItemDto { Quantity = quantity });

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Cart updated successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update cart.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(int itemId)
    {
        var client = _clientFactory.CreateClient("API");
        var userId = "demo-user"; // In a real app, get this from authentication

        var response = await client.DeleteAsync($"api/cart/{userId}/items/{itemId}");

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Item removed from cart successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to remove item from cart.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostClearCartAsync()
    {
        var client = _clientFactory.CreateClient("API");
        var userId = "demo-user"; // In a real app, get this from authentication

        var response = await client.DeleteAsync($"api/cart/{userId}");

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Cart cleared successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to clear cart.";
        }

        return RedirectToPage();
    }
} 