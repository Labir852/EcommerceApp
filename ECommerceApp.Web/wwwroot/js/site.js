// Shared API base URL
const apiBaseUrl = 'http://localhost:7241';

// Shared notification function
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = `fixed bottom-4 right-4 p-4 rounded-lg shadow-lg ${
        type === 'success' ? 'bg-green-500' : 'bg-red-500'
    } text-white z-50`;
    notification.textContent = message;
    document.body.appendChild(notification);
    setTimeout(() => notification.remove(), 3000);
}

// Initialize cart functionality if elements exist
document.addEventListener('DOMContentLoaded', () => {
   
    // Initialize cart badge
    const cartBadge = document.getElementById('cartBadge');
    if (cartBadge) {
        fetchCartCount();
    }

    // Initialize Add Product Modal
    const addProductButton = document.getElementById('addProductButton');
    const addProductModal = document.getElementById('addProductModal');
    const addProductForm = document.getElementById('addProductForm');

    if (addProductButton) {
        addProductButton.addEventListener('click', () => {
            addProductModal.classList.remove('hidden');
        });
    }

    // Close modal when clicking outside
    if (addProductModal) {
        addProductModal.addEventListener('click', (e) => {
            if (e.target === addProductModal) {
                addProductModal.classList.add('hidden');
            }
        });
    }

    // Handle form submission
    if (addProductForm) {
        addProductForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            await submitAddProduct();
        });
    }
});
fetchProducts();
// Cart functionality
async function fetchCartCount() {
    try {
        const response = await fetch(`${apiBaseUrl}/api/cart`);
        if (response.ok) {
            const cart = await response.json();
            const cartBadge = document.getElementById('cartBadge');
            const itemCount = cart.items.reduce((total, item) => total + item.quantity, 0);
            
            if (cartBadge) {
                cartBadge.textContent = itemCount;
                cartBadge.classList.toggle('hidden', itemCount === 0);
            }
        }
    } catch (error) {
        console.error('Error fetching cart count:', error);
    }
}

// Cart sidebar functionality
document.addEventListener('DOMContentLoaded', function() {
    const cartButton = document.getElementById('cartButton');
    const cartSidebar = document.getElementById('cartSidebar');
    const cartOverlay = document.getElementById('cartOverlay');
    const closeCartButton = document.getElementById('closeCartButton');

    if (cartButton && cartSidebar) {
        cartButton.addEventListener('click', function() {
            openCartSidebar();
            loadCartItems();
        });
    }

    if (closeCartButton) {
        closeCartButton.addEventListener('click', closeCartSidebar);
    }

    if (cartOverlay) {
        cartOverlay.addEventListener('click', closeCartSidebar);
    }
});


function openCartSidebar() {
    const cartSidebar = document.getElementById('cartSidebar');
    if (cartSidebar) {
        cartSidebar.classList.remove('hidden');
        document.body.style.overflow = 'hidden';
    }
}

function closeCartSidebar() {
    const cartSidebar = document.getElementById('cartSidebar');
    if (cartSidebar) {
        cartSidebar.classList.add('hidden');
        document.body.style.overflow = '';
    }
}

async function loadCartItems() {
    try {
        const response = await fetch(`${apiBaseUrl}/api/cart`);
        if (response.ok) {
            const cart = await response.json();
            updateCartSidebar(cart);
        } else {
            showNotification('Failed to load cart items', 'error');
        }
    } catch (error) {
        console.error('Error loading cart items:', error);
        showNotification('Error loading cart items', 'error');
    }
}

function updateCartSidebar(cart) {
    const cartItemsList = document.getElementById('cartItemsList');
    const cartSubtotal = document.getElementById('cartSubtotal');
    
    if (!cartItemsList || !cartSubtotal) return;
    
    if (!cart.items || cart.items.length === 0) {
        cartItemsList.innerHTML = '<p class="text-gray-500 text-center py-8">Your cart is empty</p>';
        cartSubtotal.textContent = '$0.00';
        return;
    }
    
    let subtotal = 0;
    const itemsHtml = cart.items.map(item => {
        const price = item.product.discountedPrice || item.product.price;
        const itemTotal = price * item.quantity;
        subtotal += itemTotal;
        
        return `
            <div class="flex py-6 border-b border-gray-200">
                <div class="h-24 w-24 flex-shrink-0 overflow-hidden rounded-md border border-gray-200">
                    <img src="${item.product.imageUrl}" alt="${item.product.name}" class="h-full w-full object-cover object-center">
                </div>
                <div class="ml-4 flex flex-1 flex-col">
                    <div>
                        <div class="flex justify-between text-base font-medium text-gray-900">
                            <h3>${item.product.name}</h3>
                            <p class="ml-4">$${itemTotal.toFixed(2)}</p>
                        </div>
                    </div>
                    <div class="flex flex-1 items-end justify-between text-sm">
                        <div class="flex items-center">
                            <button onclick="updateQuantity(${item.productId}, ${item.quantity - 1})" class="px-2 py-1 text-gray-600 hover:text-gray-800">-</button>
                            <span class="mx-2">Qty ${item.quantity}</span>
                            <button onclick="updateQuantity(${item.productId}, ${item.quantity + 1})" class="px-2 py-1 text-gray-600 hover:text-gray-800">+</button>
                        </div>
                        <button onclick="removeFromCart(${item.productId})" class="font-medium text-primary hover:text-blue-700">Remove</button>
                    </div>
                </div>
            </div>
        `;
    }).join('');
    
    cartItemsList.innerHTML = itemsHtml;
    cartSubtotal.textContent = `$${subtotal.toFixed(2)}`;
}

async function updateQuantity(productId, newQuantity) {
    if (newQuantity < 1) {
        await removeFromCart(productId);
        return;
    }

    try {
        const response = await fetch(`${apiBaseUrl}/api/cart/items`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                productId,
                quantity: newQuantity
            })
        });

        if (response.ok) {
            await loadCartItems();
            await fetchCartCount();
            showNotification('Cart updated successfully');
        } else {
            showNotification('Failed to update cart', 'error');
        }
    } catch (error) {
        console.error('Error updating cart:', error);
        showNotification('Error updating cart', 'error');
    }
}

async function removeFromCart(productId) {
    try {
        const response = await fetch(`${apiBaseUrl}/api/cart/items/${productId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            await loadCartItems();
            await fetchCartCount();
            showNotification('Item removed from cart');
        } else {
            showNotification('Failed to remove item from cart', 'error');
        }
    } catch (error) {
        console.error('Error removing item from cart:', error);
        showNotification('Error removing item from cart', 'error');
    }
}

// Add Product Modal functionality
function openAddProductModal() {
    const modal = document.getElementById('addProductModal');
    if (modal) {
        modal.classList.remove('hidden');
    }
}

function closeAddProductModal() {
    const modal = document.getElementById('addProductModal');
    const form = document.getElementById('addProductForm');
    if (modal) {
        modal.classList.add('hidden');
    }
    if (form) {
        form.reset();
    }
}

async function submitAddProduct() {
    const form = document.getElementById('addProductForm');
    if (!form) return;

    const formData = new FormData(form);
    const product = Object.fromEntries(formData.entries());
    
    try {
        const response = await fetch(`${apiBaseUrl}/api/products`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(product)
        });

        if (response.ok) {
            closeAddProductModal();
            showNotification('Product added successfully');
            // Refresh the products list
            if (typeof fetchProducts === 'function') {
                fetchProducts();
            }
        } else {
            showNotification('Failed to add product', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        showNotification('An error occurred while adding the product', 'error');
    }
}
