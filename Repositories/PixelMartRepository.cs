using Microsoft.EntityFrameworkCore;
using PixelMart.API.DbContexts;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Models.Product;
using PixelMart.API.ResourceParameters;
using PixelMart.API.Services;

namespace PixelMart.API.Repositories;

public class PixelMartRepository : IPixelMartRepository
{
    private readonly PixelMartDbContext _context;
    private readonly IPropertyMappingService _propertyMappingService;

    public PixelMartRepository(PixelMartDbContext context, IPropertyMappingService propertyMappingService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _propertyMappingService = propertyMappingService ??
            throw new ArgumentNullException(nameof(propertyMappingService));
    }

    #region Product

    public async Task<Product?> GetproductAsync(Guid categoryId, Guid productId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(categoryId));
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(productId));
        }

        return await _context.Products
            .Where(p => p.Id == productId && p.CategoryId == categoryId)
            .AsNoTracking()
            .FirstOrDefaultAsync() ?? null;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(categoryId));
        }

        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddProductAsync(Guid categoryId, Product product)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        ArgumentNullException.ThrowIfNull(product);

        product.CategoryId = categoryId;
        await _context.Products.AddAsync(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedList<Product>> GetProductsAsync(
        Guid categoryId,
        ProductsResourceParameters productsResourceParameters)
    {
        if (productsResourceParameters == null)
        {
            throw new ArgumentNullException(nameof(productsResourceParameters));
        }

        var productsCollection = _context.Products as IQueryable<Product>;

        if (!string.IsNullOrWhiteSpace(productsResourceParameters.SearchQuery))
        {
            var searchQuery = productsResourceParameters.SearchQuery.Trim();
            productsCollection = productsCollection
                .Where(a => a.Name.Contains(searchQuery) || a.Brand.Contains(searchQuery));
        }

        if (!string.IsNullOrWhiteSpace(productsResourceParameters.OrderBy))
        {
            // get property mapping dictionary
            var authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<ProductDto, Product>();

            productsCollection = productsCollection
                .ApplySort(productsResourceParameters.OrderBy, authorPropertyMappingDictionary);
        }

        return await PagedList<Product>.CreateAsync(productsCollection,
             productsResourceParameters.PageNumber,
             productsResourceParameters.PageSize);
    }

    public async Task<bool> ProductExistsAsync(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        return await _context.Products.AsNoTracking().AnyAsync(p => p.Id == productId);
    }

    #endregion

    #region Category

    public async Task AddCategoryAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        category.Id = Guid.NewGuid();

        if (category.Products.Any())
        {
            foreach (var product in category.Products)
            {
                product.Id = Guid.NewGuid();
            }
        }

        await _context.Categories.AddAsync(category);
    }

    public async Task<bool> CategoryExistsAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        return await _context.Categories.AsNoTracking().AnyAsync(c => c.Id == categoryId);
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        // return await _context.Categories.Include(c => c.Products).ToListAsync(); // include product details if needed
        return await _context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category?> GetCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == categoryId) ?? null;
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Stock

    public async Task<IEnumerable<Stock>> GetAllItemStocksAsync()
    {
        return await _context.Stocks.AsNoTracking().ToListAsync();
    }

    public async Task<Stock> GetItemStockAsync(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        return await _context.Stocks.AsNoTracking().FirstAsync(s => s.ProductId == productId);
    }

    public async Task AddItemStockAsync(Guid productId, Stock stock)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        ArgumentNullException.ThrowIfNull(stock);

        stock.ProductId = productId;
        await _context.Stocks.AddAsync(stock);
    }

    public async Task UpdateItemStockAsync(Guid productId, Stock itemStock)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        ArgumentNullException.ThrowIfNull(itemStock);

        itemStock.ProductId = productId;
        _context.Stocks.Update(itemStock);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> StockExistsAsync(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
        }

        return await _context.Stocks.AsNoTracking().AnyAsync(s => s.ProductId == productId);
    }

    #endregion

    #region Shopping Cart

    public async Task<IEnumerable<ShoppingCart>> GetAllCartDetailsAsync()
    {
        return await _context.ShoppingCarts.Include(sc => sc.Items).AsNoTracking().ToListAsync();
    }

    public async Task<ShoppingCart?> GetCartDetailsForUserAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        return await _context.ShoppingCarts
            .Include(sc => sc.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(sc => sc.UserId == userId.ToString()) ?? null;
    }

    public async Task AddShoppingCartAsync(Guid userId, ShoppingCart shoppingCart)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(shoppingCart);

        shoppingCart.UserId = userId.ToString();
        await _context.ShoppingCarts.AddAsync(shoppingCart);
    }

    public async Task UpdateShoppingCartAsync(Guid userId, ShoppingCart updatedShoppingCart)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(updatedShoppingCart);

        var existingCart = await _context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.ToString());

        if (existingCart == null)
            throw new InvalidOperationException("Shopping cart not found.");

        foreach (var updatedItem in updatedShoppingCart.Items)
        {
            var existingItem = existingCart.Items
                .FirstOrDefault(i => i.ProductId == updatedItem.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = updatedItem.Quantity;
            }
            else
            {
                existingCart.Items.Add(new CartItem
                {
                    ProductId = updatedItem.ProductId,
                    Quantity = updatedItem.Quantity,
                    ShoppingCartId = existingCart.Id
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteShoppingCartAsync(ShoppingCart cart)
    {
        ArgumentNullException.ThrowIfNull(cart);

        _context.ShoppingCarts.Remove(cart);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Order

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(sc => sc.Items)
            .AsNoTracking()
            .ToListAsync();

        return orders;
    }

    public async Task<IEnumerable<Order>> GetOrdersForUserAsync(Guid userId)
    {
        var userOrders = await _context.Orders
            .Include(sc => sc.Items)
            .Where(o => o.UserId == userId.ToString())
            .AsNoTracking()
            .ToListAsync();

        return userOrders;
    }

    public Task<Order?> GetOrderForUserAsync(Guid userId, Guid orderId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

        var order = _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.UserId == userId.ToString() && o.Id == orderId);

        return order ?? throw new InvalidOperationException("Order not found for user.");
    }

    public async Task CreateOrderAsync(Guid userId, Order order)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(order);

        order.UserId = userId.ToString();
        await _context.Orders.AddAsync(order);
    }

    public async Task UpdateOrderAsync(Guid userId, Guid orderId, Order orderUpdated)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

        ArgumentNullException.ThrowIfNull(orderUpdated);

        var userOrder = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.UserId == userId.ToString() && o.Id == orderId);

        if (userOrder is null)
            throw new InvalidOperationException("Order not found for user.");

        // Update scalar properties
        userOrder.Status = orderUpdated.Status;
        userOrder.ShippingAddress = orderUpdated.ShippingAddress;
        userOrder.OrderDate = DateTime.UtcNow;

        // Update or add items
        foreach (var updatedItem in orderUpdated.Items)
        {
            var existingItem = userOrder.Items
                .FirstOrDefault(i => i.ProductId == updatedItem.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = updatedItem.Quantity;
            }
            else
            {
                userOrder.Items.Add(
                    new OrderItem
                    {
                        ProductId = updatedItem.ProductId,
                        Quantity = updatedItem.Quantity,
                        OrderId = orderId
                    });
            }
        }

        // Remove items that are no longer present in the updated order
        var updatedProductIds = orderUpdated.Items
            .Select(i => i.ProductId)
            .ToHashSet();

        var itemsToRemove = userOrder.Items
            .Where(i => !updatedProductIds.Contains(i.ProductId))
            .ToList();

        foreach (var item in itemsToRemove)
        {
            userOrder.Items.Remove(item);
        }

        await _context.SaveChangesAsync();
    }

    public async Task CancelOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Common

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() >= 0; // SaveChangesAsync result contain no entries written to the DB
    }

    #endregion

}
