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
            Items = new List<CartItemDto>(),
            TotalPrice = 0,
            DiscountedTotalPrice = 0
        };

        var currentDate = DateTime.UtcNow.Date;
        foreach (var item in cart.Items)
        {
            var itemDto = new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price,
                TotalPrice = item.Product.Price * item.Quantity
            };

            if (item.Product.DiscountStartDate <= currentDate && 
                item.Product.DiscountEndDate >= currentDate)
            {
                itemDto.DiscountedUnitPrice = item.Product.Price * 
                    (1 - item.Product.DiscountPercentage / 100);
                itemDto.DiscountedTotalPrice = itemDto.DiscountedUnitPrice * item.Quantity;
            }

            cartDto.Items.Add(itemDto);
            cartDto.TotalPrice += itemDto.TotalPrice;
            if (itemDto.DiscountedTotalPrice.HasValue)
            {
                cartDto.DiscountedTotalPrice += itemDto.DiscountedTotalPrice.Value;
            }
            else
            {
                cartDto.DiscountedTotalPrice += itemDto.TotalPrice;
            }
        }

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
        }

        var product = await _context.Products.FindAsync(addToCartDto.ProductId);
        if (product == null)
        {
            return NotFound("Product not found");
        }

        if (product.StockQuantity < addToCartDto.Quantity)
        {
            return BadRequest("Not enough stock available");
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
            cart.Items.Add(new CartItem
            {
                ProductId = addToCartDto.ProductId,
                Quantity = addToCartDto.Quantity,
                UnitPrice = product.Price
            });
        }

        product.StockQuantity -= addToCartDto.Quantity;
        await _context.SaveChangesAsync();

        return await GetCart(userId);
    }

    [HttpPut("{userId}/items/{itemId}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(
        string userId, 
        int itemId, 
        UpdateCartItemDto updateCartItemDto)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart == null)
        {
            return NotFound("Cart not found");
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (cartItem == null)
        {
            return NotFound("Cart item not found");
        }

        var product = cartItem.Product;
        var quantityDifference = updateCartItemDto.Quantity - cartItem.Quantity;

        if (quantityDifference > 0 && product.StockQuantity < quantityDifference)
        {
            return BadRequest("Not enough stock available");
        }

        product.StockQuantity -= quantityDifference;
        cartItem.Quantity = updateCartItemDto.Quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetCart(userId);
    }

    [HttpDelete("{userId}/items/{itemId}")]
    public async Task<ActionResult<CartDto>> RemoveFromCart(string userId, int itemId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

        if (cart == null)
        {
            return NotFound("Cart not found");
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (cartItem == null)
        {
            return NotFound("Cart item not found");
        }

        cartItem.Product.StockQuantity += cartItem.Quantity;
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