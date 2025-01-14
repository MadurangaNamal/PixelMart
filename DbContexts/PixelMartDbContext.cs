using Microsoft.EntityFrameworkCore;

namespace PixelMart.API.DbContexts
{
    public class PixelMartDbContext : DbContext
    {
        public PixelMartDbContext(DbContextOptions<PixelMartDbContext> options) : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }
        //public DbSet<Product> Products { get; set; }
        //public DbSet<Category> Categories { get; set; }
        //public DbSet<Stock> Stocks { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    }
}
