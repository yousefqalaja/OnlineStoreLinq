using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OnlineStoreLinq;


const string connectionString =
    @"Server=(localdb)\MSSQLLocalDB;Database=OnlineStoreLinq;Trusted_Connection=True;TrustServerCertificate=True";

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;

using var db = new AppDbContext(options);
db.Database.EnsureCreated();
DbSeeder.Seed(db);

var s = new Solutions(db);
var json = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles };

async Task Run<T>(string title, Func<Task<T>> query)
{
    Console.WriteLine($"\n=== {title} ===");
    var result = await query();
    if (result is System.Collections.IEnumerable list && result is not string)
    {
        int count = 0;
        foreach (var item in list)
            if (count++ < 3) Console.WriteLine(JsonSerializer.Serialize(item, item.GetType(), json));
        Console.WriteLine($"({count} rows)");
    }
    else Console.WriteLine(JsonSerializer.Serialize(result, json));
}

// Level 1
await Run("Q1  Delivered orders totals", s.Q1);
await Run("Q2  Orders per status %", s.Q2);
await Run("Q3  Price above category avg", s.Q3);
await Run("Q4  Never ordered products", s.Q4);
await Run("Q5  New customers with orders", s.Q5);
await Run("Q6  Cancelled > 2", s.Q6);

// Level 2
await Run("Q7  Top 5 customers", s.Q7);
await Run("Q8  Monthly report 2025", s.Q8);
await Run("Q9  Revenue per category", s.Q9);
await Run("Q10 Customers w/o orders by city", s.Q10);
await Run("Q11 Highly rated products", s.Q11);
await Run("Q12 Top 2 expensive per category", s.Q12);
await Run("Q13 Stock < pending qty", s.Q13);
await Run("Q14 Above-average spenders", s.Q14);

// Level 3
await Run("Q15 Best seller per category", s.Q15);
await Run("Q16 Bought every laptop", s.Q16);
await Run("Q17 Reviews without purchase", s.Q17);
await Run("Q18 Customers + order count", s.Q18);
await Run("Q19 Top 5 product pairs", s.Q19);
await Run("Q20 Last order per customer", s.Q20);
await Run("Q21 Favorite category", s.Q21);
await Run("Q22 Product revenue rank", s.Q22);

// Level 4
await Run("Q23 SearchProducts", () => s.SearchProducts(new ProductFilter
{
    Text = "Model",
    MinPrice = 100,
    InStockOnly = true,
    SortBy = ProductSort.Price,
    Descending = true,
    Page = 1,
    PageSize = 5
}));

// Bonus
Console.WriteLine("\n=== Bonus (a): SQL of Q8 ===\n" + s.BonusA());

await Run("Q24 Accessories price +10% (rows affected)", s.Q24);
await Run("Q25 Delete old cancelled orders (rows affected)", s.Q25);
