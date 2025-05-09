using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PixelMart.API.Entities;

namespace PixelMart.API.DbContexts;

public class PixelMartDbContext : IdentityDbContext<ApplicationUser>
{
    public PixelMartDbContext(DbContextOptions<PixelMartDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShoppingCart>()
            .HasMany(sc => sc.Items)
            .WithOne(ci => ci.ShoppingCart)
            .HasForeignKey(ci => ci.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShoppingCart>()
            .HasOne(sc => sc.User)
            .WithOne(u => u.ShoppingCart)
            .HasForeignKey<ShoppingCart>(sc => sc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Stock)
            .WithOne(s => s.Product)
            .HasForeignKey<Stock>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .Property(p => p.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(p => p.UnitPrice)
            .HasPrecision(18, 2);

    }
}
