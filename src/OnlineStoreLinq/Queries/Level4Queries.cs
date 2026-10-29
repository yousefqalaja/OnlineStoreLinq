using Microsoft.EntityFrameworkCore;

namespace OnlineStoreLinq;

public partial class Solutions
{
    // ---------------------------------------------------------------
    // Level 4
    // ---------------------------------------------------------------

    // 23
    public async Task<PagedResult<ProductListItem>> SearchProducts(ProductFilter f)
    {
        IQueryable<Product> q = db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(f.Text))
            q = q.Where(p => p.Name.Contains(f.Text));
        if (f.CategoryId.HasValue)
            q = q.Where(p => p.CategoryId == f.CategoryId.Value);
        if (f.MinPrice.HasValue)
            q = q.Where(p => p.Price >= f.MinPrice.Value);
        if (f.MaxPrice.HasValue)
            q = q.Where(p => p.Price <= f.MaxPrice.Value);
        if (f.InStockOnly)
            q = q.Where(p => p.Stock > 0);

        var total = await q.CountAsync();         // count BEFORE paging

        q = (f.SortBy, f.Descending) switch
        {
            (ProductSort.Price, false)  => q.OrderBy(p => p.Price),
            (ProductSort.Price, true)   => q.OrderByDescending(p => p.Price),
            (ProductSort.Rating, false) => q.OrderBy(p => p.Reviews.Average(r => (double?)r.Rating)),
            (ProductSort.Rating, true)  => q.OrderByDescending(p => p.Reviews.Average(r => (double?)r.Rating)),
            (_, true)                   => q.OrderByDescending(p => p.Name),
            _                           => q.OrderBy(p => p.Name)
        };

        var page = Math.Max(1, f.Page);
        var size = Math.Clamp(f.PageSize, 1, 100);

        var items = await ((IOrderedQueryable<Product>)q)
            .ThenBy(p => p.Id)                    // stable paging
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new ProductListItem(
                p.Id, p.Name, p.Category.Name, p.Price, p.Stock,
                p.Reviews.Average(r => (double?)r.Rating)))
            .ToListAsync();

        return new PagedResult<ProductListItem>(items, total, page, size);
    }

    // 24
    public Task<int> Q24() =>
        db.Products
          .Where(p => p.Category.Name == "Accessories" && p.Stock < 5)
          .ExecuteUpdateAsync(s => s.SetProperty(p => p.Price, p => p.Price * 1.10m));

    // 25
    public Task<int> Q25()
    {
        var cutoff = DateTime.UtcNow.AddYears(-1);
        return db.Orders
            .Where(o => o.Status == OrderStatus.Cancelled && o.OrderDate < cutoff)
            .ExecuteDeleteAsync();
        // OrderItems are removed by the database ON DELETE CASCADE (Order -> Items)
    }
}
