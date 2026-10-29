using Microsoft.EntityFrameworkCore;

namespace OnlineStoreLinq;

public partial class Solutions
{
    // ---------------------------------------------------------------
    // Bonus
    // ---------------------------------------------------------------

    // (a) SQL of question 8
    public string BonusA() => Q8Query().ToQueryString();

    // (b) N+1 problem
    // Q20 written WRONG (N+1):
    public async Task BonusB_NPlusOne()
    {
        var customers = await db.Customers.ToListAsync();                 // 1 query

        foreach (var c in customers)                                      // N customers
        {
            var last = await db.Orders                                    // +1 query per customer
                .Where(o => o.CustomerId == c.Id)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            if (last == null) continue;

            var items = await db.OrderItems                               // +1 more per customer
                .Where(i => i.OrderId == last.Id)
                .Include(i => i.Product)
                .ToListAsync();
        }
        // 1 + N (+ N) round trips. With lazy loading it is even sneakier:
        // c.Orders / o.Items / i.Product silently fire a query on every access.
        // Fix: the single projection in Q20 (or Include / AsSplitQuery).
    }
}

// =====================================================================
//  (c) Why Select((x, i) => ...) fails in Q22
//  SQL is set-based: rows have no inherent index/position, so EF Core has
//  no translation for the (item, index) overload of Select.
//  Fix options:
//    1) ToListAsync() first, then Select((x, i) => ...) in memory (used in Q22).
//    2) Let SQL compute it with ROW_NUMBER()/RANK() via FromSql / raw SQL.
//    3) Compute rank by counting: rank = 1 + count(products with higher revenue)
//       (also handles ties properly).
// =====================================================================
