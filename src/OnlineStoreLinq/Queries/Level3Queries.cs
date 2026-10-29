using Microsoft.EntityFrameworkCore;

namespace OnlineStoreLinq;

public partial class Solutions
{
    // ---------------------------------------------------------------
    // Level 3
    // ---------------------------------------------------------------

    // 15  (categories with no sales => BestSeller = null)
    public Task<List<object>> Q15() =>
        db.Categories
          .Select(c => (object)new
          {
              Category = c.Name,
              BestSeller = c.Products
                  .Select(p => new
                  {
                      p.Name,
                      Qty = p.OrderItems
                              .Where(i => i.Order.Status == OrderStatus.Delivered)
                              .Sum(i => i.Quantity)
                  })
                  .Where(x => x.Qty > 0)
                  .OrderByDescending(x => x.Qty)
                  .FirstOrDefault()
          })
          .ToListAsync();

    // 16  (relational division: no laptop exists that the customer did NOT buy)
    public async Task<List<Customer>> Q16()
    {
        var hasLaptops = await db.Products.AnyAsync(p => p.Category.Name == "Laptops");
        if (!hasLaptops) return new();

        return await db.Customers
            .Where(c => db.Products
                .Where(p => p.Category.Name == "Laptops")
                .All(p => c.Orders.Any(o =>
                    o.Status == OrderStatus.Delivered &&
                    o.Items.Any(i => i.ProductId == p.Id))))
            .ToListAsync();
    }

    // 17
    public Task<List<Review>> Q17() =>
        db.Reviews
          .Where(r => !db.OrderItems.Any(i =>
              i.ProductId == r.ProductId &&
              i.Order.CustomerId == r.CustomerId &&
              i.Order.Status == OrderStatus.Delivered))
          .ToListAsync();

    // 18  (GroupJoin => LEFT JOIN)
    public Task<List<CustomerOrderCountDto>> Q18() =>
        db.Customers
          .GroupJoin(db.Orders,
              c => c.Id,
              o => o.CustomerId,
              (c, orders) => new CustomerOrderCountDto(c.FullName, orders.Count()))
          .ToListAsync();

    // 19  (self-join on OrderItems; a.ProductId < b.ProductId avoids (A,B) and (B,A) duplicates)
    public Task<List<ProductPairDto>> Q19() =>
        (from a in db.OrderItems
         join b in db.OrderItems on a.OrderId equals b.OrderId
         where a.ProductId < b.ProductId
         group 1 by new
         {
             AId = a.ProductId, AName = a.Product.Name,
             BId = b.ProductId, BName = b.Product.Name
         } into g
         orderby g.Count() descending
         select new ProductPairDto(g.Key.AName, g.Key.BName, g.Count()))
        .Take(5)
        .ToListAsync();

    // 20  (one query: customer + latest order + its items)
    public Task<List<object>> Q20() =>
        db.Customers
          .Select(c => (object)new
          {
              c.FullName,
              LastOrder = c.Orders
                  .OrderByDescending(o => o.OrderDate)
                  .Select(o => new
                  {
                      o.Id,
                      o.OrderDate,
                      Items = o.Items.Select(i => new
                      {
                          ProductName = i.Product.Name,
                          i.Quantity
                      })
                  })
                  .FirstOrDefault()
          })
          .AsSplitQuery()
          .ToListAsync();

    // 21  (aggregate in SQL, pick the max per customer in memory)
    public async Task<List<object>> Q21()
    {
        var sums = await db.OrderItems
            .Where(i => i.Order.Status == OrderStatus.Delivered)
            .GroupBy(i => new
            {
                i.Order.CustomerId,
                Customer = i.Order.Customer.FullName,
                Category = i.Product.Category.Name
            })
            .Select(g => new
            {
                g.Key.CustomerId,
                g.Key.Customer,
                g.Key.Category,
                Qty = g.Sum(i => i.Quantity)
            })
            .ToListAsync();

        return sums
            .GroupBy(x => x.CustomerId)
            .Select(g => g.OrderByDescending(x => x.Qty).First())
            .Select(x => (object)new { x.Customer, FavoriteCategory = x.Category, x.Qty })
            .ToList();
    }

    // 22  (Select with index can't be translated => materialize first)
    public async Task<List<RankDto>> Q22()
    {
        var revenues = await db.OrderItems
            .Where(i => i.Order.Status == OrderStatus.Delivered)
            .GroupBy(i => i.Product.Name)
            .Select(g => new
            {
                Product = g.Key,
                Revenue = g.Sum(i => i.Quantity * i.UnitPrice * (1 - i.Discount))
            })
            .OrderByDescending(x => x.Revenue)
            .ToListAsync();                       // SQL ends here

        return revenues
            .Select((x, i) => new RankDto(i + 1, x.Product, x.Revenue))   // LINQ to Objects
            .ToList();
    }
}
