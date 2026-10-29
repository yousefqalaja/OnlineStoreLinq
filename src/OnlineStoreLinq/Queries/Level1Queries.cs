using Microsoft.EntityFrameworkCore;

namespace OnlineStoreLinq;

public partial class Solutions
{
    // ---------------------------------------------------------------
    // Level 1
    // ---------------------------------------------------------------

    // 1
    public Task<List<OrderTotalDto>> Q1() =>
        db.Orders
          .Where(o => o.Status == OrderStatus.Delivered)
          .Select(o => new OrderTotalDto(
              o.Id,
              o.Customer.FullName,
              o.Items.Sum(i => i.Quantity * i.UnitPrice * (1 - i.Discount))))
          .OrderByDescending(x => x.Total)
          .ToListAsync();

    // 2
    public async Task<object> Q2()
    {
        var totalOrders = await db.Orders.CountAsync();
        return await db.Orders
            .GroupBy(o => o.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count(),
                Percentage = g.Count() * 100.0 / totalOrders
            })
            .ToListAsync();
    }

    // 3
    public Task<List<Product>> Q3() =>
        db.Products
          .Where(p => p.Price > db.Products
              .Where(x => x.CategoryId == p.CategoryId)
              .Average(x => x.Price))
          .ToListAsync();

    // 4
    public Task<List<Product>> Q4() =>
        db.Products.Where(p => !p.OrderItems.Any()).ToListAsync();

    // 5
    public Task<List<Customer>> Q5()
    {
        var since = DateTime.UtcNow.AddDays(-30);
        return db.Customers
            .Where(c => c.RegisteredAt >= since && c.Orders.Any())
            .ToListAsync();
    }

    // 6
    public Task<List<CancelledDto>> Q6() =>
        db.Customers
          .Select(c => new CancelledDto(
              c.FullName,
              c.Orders.Count(o => o.Status == OrderStatus.Cancelled)))
          .Where(x => x.CancelledCount > 2)
          .ToListAsync();
}
