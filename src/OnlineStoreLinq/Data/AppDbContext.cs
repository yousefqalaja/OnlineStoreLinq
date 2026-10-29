using Microsoft.EntityFrameworkCore;

namespace OnlineStoreLinq;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        b.Entity<OrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
        b.Entity<OrderItem>().Property(i => i.Discount).HasPrecision(5, 4);

        b.Entity<Order>().Property(o => o.Status).HasConversion<string>();

        // Avoid SQL Server "multiple cascade paths" errors
        b.Entity<Review>().HasOne(r => r.Customer).WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<OrderItem>().HasOne(i => i.Product).WithMany(p => p.OrderItems)
            .HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);

        // OrderItem -> Order stays CASCADE (needed for Q25 ExecuteDelete)
    }
}
