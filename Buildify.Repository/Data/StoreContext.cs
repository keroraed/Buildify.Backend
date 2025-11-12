using Buildify.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Buildify.Repository.Data;

public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions<StoreContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(p => p.Description)
                .HasMaxLength(1000);
            
            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            entity.Property(p => p.Stock)
                .IsRequired();
            
            entity.Property(p => p.ImageUrl)
                .HasMaxLength(500);
            
            entity.Property(p => p.CreatedDate)
                .IsRequired();
            
            // Configure relationship
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(c => c.Description)
                .HasMaxLength(500);
        });

        // Configure Cart entity
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.UserId)
                .IsRequired()
                .HasMaxLength(450);
            
            entity.Property(c => c.CreatedDate)
                .IsRequired();

            entity.HasIndex(c => c.UserId);
        });

        // Configure CartItem entity
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            
            entity.Property(ci => ci.Quantity)
                .IsRequired();
            
            entity.Property(ci => ci.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            // Configure relationship with Cart
            entity.HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure relationship with Product
            entity.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            
            entity.Property(o => o.UserId)
                .IsRequired()
                .HasMaxLength(450);
            
            entity.Property(o => o.OrderDate)
                .IsRequired();
            
            entity.Property(o => o.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            entity.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>();

            // Shipping address properties
            entity.Property(o => o.ShippingFirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(o => o.ShippingLastName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(o => o.ShippingStreet)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(o => o.ShippingCity)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(o => o.ShippingState)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(o => o.ShippingZipCode)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.Property(o => o.ShippingCountry)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(o => o.UserId);
            entity.HasIndex(o => o.OrderDate);
            entity.HasIndex(o => o.Status);
        });

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            
            entity.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(oi => oi.ProductImageUrl)
                .HasMaxLength(500);
            
            entity.Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            entity.Property(oi => oi.Quantity)
                .IsRequired();
            
            // Configure relationship with Order
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Configure relationship with Product
            entity.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
