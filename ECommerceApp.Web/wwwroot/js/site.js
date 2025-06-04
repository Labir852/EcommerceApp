// Shared API base URL
const apiBaseUrl = 'http://localhost:7241';

// Get or generate userId
let userId = localStorage.getItem('userId') || generateUserId();
let debounceTimeout;

function generateUserId() {
    const newUserId = 'user_' + Math.random().toString(36).substr(2, 9);
    localStorage.setItem('userId', newUserId);
    return newUserId;
}

// Initialize search functionality
document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            clearTimeout(debounceTimeout);
            debounceTimeout = setTimeout(() => {
                fetchProducts(e.target.value);
            }, 300);
        });
    }
    
    // Initial products load
    fetchProducts();
    fetchCartCount();

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

console.log('site.js loaded, API Base URL:', apiBaseUrl);

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

// Products functionality
async function fetchProducts(searchTerm = '') {
    console.log('Fetching products...');
    const params = new URLSearchParams({
        searchTerm: searchTerm || '',
        page: '1',
        pageSize: '12'
    });
    
    const grid = document.getElementById('productsGrid');
    if (!grid) {
        console.error('Products grid element not found!');
        return;
    }

    grid.innerHTML = '<div class="text-center text-gray-500 col-span-full">Loading products...</div>';

    try {
        const url = `${apiBaseUrl}/api/products?${params}`;
        console.log('Making API request to:', url);
        
        const response = await fetch(url);
        console.log('Response status:', response.status);
        
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
        }
        
        const data = await response.json();
        console.log('Products data:', data);

        if (!data) {
            throw new Error('No data received from API');
        }

        // Handle both array and object with items property
        const products = data.items || data;
        if (!Array.isArray(products)) {
            throw new Error('Invalid data format received');
        }

        updateProductsGrid(products);
    } catch (error) {
        console.error('Error fetching products:', error);
        grid.innerHTML = `
            <div class="text-center text-red-500 col-span-full">
                <p>Error loading products</p>
                <p class="text-sm mt-2">${error.message}</p>
                <button onclick="fetchProducts()" class="mt-4 text-white bg-primary px-4 py-2 rounded">
                    Try Again
                </button>
            </div>
        `;
    }
}

function updateProductsGrid(products) {
    console.log('Starting to update products grid with', products.length, 'products');
    const grid = document.getElementById('productsGrid');
    
    if (!grid) {
        console.error('Products grid element not found in updateProductsGrid');
        return;
    }

    if (products.length === 0) {
        console.log('No products to display');
        grid.innerHTML = '<div class="text-gray-500 text-center py-8 col-span-full">No products found</div>';
        return;
    }

    console.log('Building product grid HTML');
    const productsHtml = products.map(product => {
        console.log('Processing product:', product);
        return `
            <div class="group relative bg-white p-4 rounded-lg shadow-sm">
                <div class="aspect-h-1 aspect-w-1 w-full overflow-hidden rounded-md bg-gray-200 lg:aspect-none group-hover:opacity-75 lg:h-80">
                    <img src="${product.imageUrl || ''}" 
                         alt="${product.name || 'Product'}"
                         class="h-full w-full object-cover object-center lg:h-full lg:w-full">
                </div>
                <div class="mt-4 flex justify-between">
                    <div>
                        <h3 class="text-sm font-medium text-gray-900">
                            ${product.name || 'Unnamed Product'}
                        </h3>
                        <p class="mt-1 text-sm text-gray-500">${product.description || 'No description available'}</p>
                    </div>
                    <div class="text-right">
                        <p class="text-sm font-medium text-gray-900">
                            ${product.discountedPrice 
                                ? `<span class="line-through text-gray-500">$${(product.price || 0).toFixed(2)}</span><br>
                                   <span class="text-red-600">$${product.discountedPrice.toFixed(2)}</span>`
                                : `$${(product.price || 0).toFixed(2)}`}
                        </p>
                        ${(product.stockQuantity <= 5 && product.stockQuantity > 0)
                            ? `<p class="text-xs text-red-500 mt-1">Only ${product.stockQuantity} left!</p>`
                            : ''}
                    </div>
                </div>
                <button type="button" 
                        onclick="addToCart(${product.id})"
                        class="mt-4 flex w-full items-center justify-center rounded-md border border-transparent bg-primary px-8 py-2 text-base font-medium text-white hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 ${(!product.stockQuantity || product.stockQuantity <= 0) ? 'opacity-50 cursor-not-allowed' : ''}"
                        ${(!product.stockQuantity || product.stockQuantity <= 0) ? 'disabled' : ''}>
                    <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                    ${(!product.stockQuantity || product.stockQuantity <= 0) ? 'Out of Stock' : 'Add to Cart'}
                </button>
            </div>
        `;
    }).join('');

    console.log('Setting grid HTML');
    grid.innerHTML = productsHtml;
    console.log('Grid update complete');
}

// Cart functionality
async function addToCart(productId) {
    try {
        const response = await fetch(`${apiBaseUrl}/api/cart/${userId}/items`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                userId: userId,
                productId: productId,
                quantity: 1
            })
        });
        
        if (response.ok) {
            showNotification('Product added to cart!', 'success');
            await fetchCartCount(); // Update cart count after adding item
        } else {
            const errorData = await response.text();
            showNotification(`Failed to add product to cart: ${errorData}`, 'error');
        }
    } catch (error) {
        console.error('Error adding product to cart:', error);
        showNotification('An error occurred while adding to cart', 'error');
    }
}

async function fetchCartCount() {
    try {
        const response = await fetch(`${apiBaseUrl}/api/cart/${userId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        if (response.ok) {
            const cart = await response.json();
            const cartBadge = document.getElementById('cartBadge');
            const itemCount = cart.items.reduce((total, item) => total + item.quantity, 0);
            
            if (cartBadge) {
                cartBadge.textContent = itemCount;
                cartBadge.classList.toggle('hidden', itemCount === 0);
            }
        } else {
            console.error('Failed to fetch cart:', await response.text());
        }
    } catch (error) {
        console.error('Error fetching cart:', error);
    }
}

// Cart sidebar functionality
function openCartSidebar() {
    const cartSidebar = document.getElementById('cartSidebar');
    const cartOverlay = document.getElementById('cartOverlay');
    if (cartSidebar && cartOverlay) {
        cartOverlay.classList.remove('hidden');
        cartSidebar.classList.remove('translate-x-full');
        document.body.style.overflow = 'hidden';
        loadCartItems(); // Load cart items when opening sidebar
    }
}

function closeCartSidebar() {
    const cartSidebar = document.getElementById('cartSidebar');
    const cartOverlay = document.getElementById('cartOverlay');
    if (cartSidebar && cartOverlay) {
        cartOverlay.classList.add('hidden');
        cartSidebar.classList.add('translate-x-full');
        document.body.style.overflow = '';
    }
}

async function loadCartItems() {
    try {
        const response = await fetch(`${apiBaseUrl}/api/cart/${userId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });
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
    const itemsHtml = cart.items.map(item =>
    {
        const price = item.unitPrice;
        const itemTotal = price * item.quantity;
        subtotal += itemTotal;
        
        return `
            <div class="flex py-6 border-b border-gray-200">
                <div class="h-24 w-24 flex-shrink-0 overflow-hidden rounded-md border border-gray-200">
                    <img src="${item?.imageUrl}" alt="${item.productName}" class="h-full w-full object-cover object-center">
                </div>
                <div class="ml-4 flex flex-1 flex-col">
                    <div>
                        <div class="flex justify-between text-base font-medium text-gray-900">
                            <h3>${item.productName}</h3>
                            <p class="ml-4">$${itemTotal.toFixed(2)}</p>
                        </div>
                        <p class="mt-1 text-sm text-gray-500">${item?.description}</p>
                    </div>
                    <div class="flex flex-1 items-end justify-between text-sm">
                        <div class="flex items-center">
                            <button data-action="decrease" data-product-id="${item.productId}" data-quantity="${item.quantity}" class="px-2 py-1 text-gray-600 hover:text-gray-800 btn btn-danger">-</button>
                            <span class="mx-2">Qty ${item.quantity}</span>
                            <button data-action="increase" data-product-id="${item.productId}" data-quantity="${item.quantity}" class="px-2 py-1 text-gray-600 hover:text-gray-800 btn btn-primary">+</button>
                        </div>
                        <button data-action="remove" data-product-id="${item.productId}" class="font-medium text-primary hover:text-blue-700 btn btn-danger">Remove</button>
                    </div>
                </div>
            </div>
        `;
    }).join('');
    
    cartItemsList.innerHTML = itemsHtml;
    cartSubtotal.textContent = `$${subtotal.toFixed(2)}`;

    // Add event listeners to the buttons
    cartItemsList.querySelectorAll('button[data-action]').forEach(button => {
        button.addEventListener('click', (e) => {
            const action = button.dataset.action;
            const productId = parseInt(button.dataset.productId);
            const quantity = parseInt(button.dataset.quantity);

            switch(action) {
                case 'decrease':
                    updateQuantity(productId, quantity - 1);
                    break;
                case 'increase':
                    updateQuantity(productId, quantity + 1);
                    break;
                case 'remove':
                    removeFromCart(productId);
                    break;
            }
        });
    });
}

async function updateQuantity(productId, newQuantity) {
    if (newQuantity < 1) {
        await removeFromCart(productId);
        return;
    }

    try {
        const response = await fetch(`${apiBaseUrl}/api/cart/${userId}/items/${productId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                quantity: newQuantity
            })
        });

        if (response.ok) {
            await loadCartItems();
            await fetchCartCount();
            showNotification('Cart updated successfully');
        } else {
            const errorText = await response.text();
            showNotification(`Failed to update cart: ${errorText}`, 'error');
        }
    } catch (error) {
        console.error('Error updating cart:', error);
        showNotification('Error updating cart', 'error');
    }
}

async function removeFromCart(productId) {
    try {
        const response = await fetch(`${apiBaseUrl}/api/cart/${userId}/items/${productId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            await loadCartItems();
            await fetchCartCount();
            showNotification('Item removed from cart');
        } else {
            const errorText = await response.text();
            showNotification(`Failed to remove item from cart: ${errorText}`, 'error');
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

function proceedToCheckout() {
    showNotification('Checkout functionality coming soon!', 'info');
}

// Initialize cart when page loads
document.addEventListener('DOMContentLoaded', () => {
    fetchProducts();
    fetchCartCount();

    // Add click event listener for cart button
    const cartButton = document.getElementById('cartButton');
    if (cartButton) {
        cartButton.addEventListener('click', openCartSidebar);
    }
});

// Add your JavaScript code here
