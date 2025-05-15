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
    public async Task<Product> GetproductAsync(Guid categoryId, Guid productId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(categoryId));
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(productId));
        }

#pragma warning disable CS8603 // Possible null reference return.
        return await _context.Products
            .Where(p => p.Id == productId && p.CategoryId == categoryId)
            .AsNoTracking()
            .FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.

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
            productsCollection = productsCollection.Where(a => a.Name.Contains(searchQuery) || a.Brand.Contains(searchQuery));
        }

        if (!string.IsNullOrWhiteSpace(productsResourceParameters.OrderBy))
        {
            // get property mapping dictionary
            var authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<ProductDto, Product>();

#pragma warning disable S1854 // Unused assignments should be removed
            productsCollection = productsCollection.ApplySort(productsResourceParameters.OrderBy, authorPropertyMappingDictionary);
#pragma warning restore S1854 // Unused assignments should be removed

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
        //return await _context.Categories.Include(c => c.Products).ToListAsync(); //include product details if need
        return await _context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category> GetCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
        }

#pragma warning disable CS8603 // Possible null reference return.
        return await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == categoryId);
#pragma warning restore CS8603 // Possible null reference return.

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
        return await _context.ShoppingCarts.AsNoTracking().ToListAsync();
    }

    public async Task<ShoppingCart> GetCartDetailsForUserAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        return await _context.ShoppingCarts.AsNoTracking().FirstAsync(sc => sc.UserId.Equals(userId.ToString()));
    }

    public async Task AddShoppingCartAsync(Guid userId, ShoppingCart shoppingCart)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(shoppingCart);
        shoppingCart.UserId = userId.ToString();
        await _context.ShoppingCarts.AddAsync(shoppingCart);
    }

    public async Task UpdateShoppingCartAsync(Guid userId, ShoppingCart shoppingCart)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ArgumentNullException.ThrowIfNull(shoppingCart);
        shoppingCart.UserId = userId.ToString();
        _context.ShoppingCarts.Update(shoppingCart);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteShoppingCartAsync(ShoppingCart cart)
    {
        ArgumentNullException.ThrowIfNull(cart);
        _context.ShoppingCarts.Remove(cart);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Common
    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    #endregion
}
