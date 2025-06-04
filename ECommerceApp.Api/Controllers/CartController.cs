using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Api.Data;
using ECommerceApp.Api.Models;
using ECommerceApp.Api.Models.DTOs;

namespace ECommerceApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<CartDto>> GetCart(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var cartDto = new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.Items.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.UnitPrice * item.Quantity,
                DiscountedUnitPrice = item.DiscountedUnitPrice,
                DiscountedTotalPrice = item.DiscountedUnitPrice.HasValue ? 
                    item.DiscountedUnitPrice.Value * item.Quantity : null
            }).ToList()
        };

        cartDto.TotalPrice = cartDto.Items.Sum(i => i.TotalPrice);
        cartDto.DiscountedTotalPrice = cartDto.Items
            .Sum(i => i.DiscountedTotalPrice ?? i.TotalPrice);

        return cartDto;
    }

    [HttpPost("{userId}/items")]
    public async Task<ActionResult<CartDto>> AddToCart(string userId, AddToCartDto addToCartDto)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var product = await _context.Products.FindAsync(addToCartDto.ProductId);
        if (product == null)
        {
            return NotFound("Product not found");
        }

        var existingItem = cart.Items
            .FirstOrDefault(i => i.ProductId == addToCartDto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += addToCartDto.Quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = addToCartDto.ProductId,
                Quantity = addToCartDto.Quantity,
                UnitPrice = product.Price,
                DiscountedUnitPrice = product.DiscountedPrice
            };
            _context.CartItems.Add(newItem);
        }

        await _context.SaveChangesAsync();
        return await GetCart(userId);
    }

    [HttpPut("{userId}/items/{productId}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(string userId, int productId, UpdateCartItemDto updateCartItemDto)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart == null)
        {
            return NotFound("Cart not found");
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (cartItem == null)
        {
            return NotFound("Cart item not found");
        }

        cartItem.Quantity = updateCartItemDto.Quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetCart(userId);
    }

    [HttpDelete("{userId}/items/{productId}")]
    public async Task<ActionResult<CartDto>> RemoveFromCart(string userId, int productId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart == null)
        {
            return NotFound("Cart not found");
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (cartItem == null)
        {
            return NotFound("Cart item not found");
        }

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return await GetCart(userId);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> ClearCart(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart == null)
        {
            return NotFound("Cart not found");
        }

        foreach (var item in cart.Items)
        {
            item.Product.StockQuantity += item.Quantity;
        }

        cart.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
} 