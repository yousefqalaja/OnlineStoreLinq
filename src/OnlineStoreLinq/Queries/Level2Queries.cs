using Microsoft.EntityFrameworkCore;

namespace OnlineStoreLinq;

public partial class Solutions
{
    // ---------------------------------------------------------------
    // Level 2
    // ---------------------------------------------------------------

    // 7  (start from OrderItems: SQL Server can't aggregate over a sub-query)
    public Task<List<object>> Q7() =>
        db.OrderItems
          .Where(i => i.Order.Status == OrderStatus.Delivered)
          .GroupBy(i => new { i.Order.CustomerId, i.Order.Customer.FullName })
          .Select(g => new
          {
              Name = g.Key.FullName,
              OrderCount = g.Select(i => i.OrderId).Distinct().Count(),
              TotalSpent = g.Sum(i => i.Quantity * i.UnitPrice * (1 - i.Discount)),
              AvgOrderValue = g.Sum(i => i.Quantity * i.UnitPrice * (1 - i.Discount))
                              / g.Select(i => i.OrderId).Distinct().Count()
          })
          .OrderByDescending(x => x.TotalSpent)
          .Take(5)
          .Select(x => (object)x)
          .ToListAsync();

    // 8  (IQueryable so we can call ToQueryString() in Bonus a)
    public IQueryable<MonthlyReportDto> Q8Query() =>
        db.OrderItems
          .Where(i => i.Order.Status == OrderStatus.Delivered
                   && i.Order.OrderDate.Year == 2025)
          .GroupBy(i => new { i.Order.OrderDate.Year, i.Order.OrderDate.Month })
          .Select(g => new MonthlyReportDto(
              g.Key.Year,
              g.Key.Month,
              g.Select(i => i.OrderId).Distinct().Count(),
              g.Sum(i => i.Quantity * i.UnitPrice * (1 - i.Discount))))
          .OrderBy(x => x.Year).ThenBy(x => x.Month);

    public Task<List<MonthlyReportDto>> Q8() => Q8Query().ToListAsync();

    // 9
    public Task<List<CategoryRevenueDto>> Q9() =>
        db.OrderItems
          .Where(i => i.Order.Status == OrderStatus.Delivered)
          .GroupBy(i => i.Product.Category.Name)
          .Select(g => new CategoryRevenueDto(
              g.Key,
              g.Sum(i => i.Quantity * i.UnitPrice * (1 - i.Discount))))
          .OrderByDescending(x => x.Revenue)
          .ToListAsync();

    // 10
    public Task<List<CityCountDto>> Q10() =>
        db.Customers
          .Where(c => !c.Orders.Any())
          .GroupBy(c => c.City)
          .Select(g => new CityCountDto(g.Key, g.Count()))
          .OrderByDescending(x => x.Count)
          .ToListAsync();

    // 11
    public Task<List<object>> Q11() =>
        db.Products
          .Where(p => p.Reviews.Count >= 10 && p.Reviews.Average(r => r.Rating) >= 4.5)
          .Select(p => (object)new
          {
              p.Name,
              Category = p.Category.Name,
              ReviewCount = p.Reviews.Count,
              AvgRating = p.Reviews.Average(r => r.Rating)
          })
          .ToListAsync();

    // 12
    public Task<List<object>> Q12() =>
        db.Categories
          .Select(c => (object)new
          {
              Category = c.Name,
              Top2 = c.Products
                      .OrderByDescending(p => p.Price)
                      .Take(2)
                      .Select(p => new { p.Name, p.Price })
                      .ToList()
          })
          .ToListAsync();

    // 13
    public Task<List<object>> Q13() =>
        db.Products
          .Select(p => new
          {
              p.Name,
              p.Stock,
              PendingQty = p.OrderItems
                  .Where(i => i.Order.Status == OrderStatus.Pending)
                  .Sum(i => i.Quantity)
          })
          .Where(x => x.Stock < x.PendingQty)
          .Select(x => (object)x)
          .ToListAsync();

    // 14
    public async Task<List<CustomerSpendDto>> Q14()
    {
        var spending = db.Customers.Select(c => new CustomerSpendDto(
            c.FullName,
            c.Orders
             .Where(o => o.Status == OrderStatus.Delivered)
             .SelectMany(o => o.Items)
             .Sum(i => i.Quantity * i.UnitPrice * (1 - i.Discount))));

        var avg = await spending.AverageAsync(x => x.Spent);   // average over ALL customers
        return await spending.Where(x => x.Spent > avg).ToListAsync();
    }
}
