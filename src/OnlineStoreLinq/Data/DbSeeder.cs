namespace OnlineStoreLinq;

/// <summary>Fake data so every query returns something. Fixed seed = repeatable results.</summary>
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Customers.Any()) return;

        var rnd = new Random(42);
        var now = DateTime.UtcNow;

        var cats = new[] { "Laptops", "Accessories", "Phones", "Monitors" }
            .Select(n => new Category { Name = n }).ToList();
        db.Categories.AddRange(cats);

        var products = new List<Product>();
        foreach (var c in cats)
            for (int i = 1; i <= 6; i++)
                products.Add(new Product
                {
                    Name = $"{c.Name} Model {i}",
                    Price = rnd.Next(10, 2000),
                    Stock = rnd.Next(0, 30),
                    Category = c
                });
        db.Products.AddRange(products);

        var cities = new[] { "Cairo", "Alexandria", "Giza", "Aswan" };
        var customers = Enumerable.Range(1, 40).Select(i => new Customer
        {
            FullName = $"Customer {i}",
            Email = $"customer{i}@mail.com",
            City = cities[rnd.Next(cities.Length)],
            RegisteredAt = now.AddDays(-rnd.Next(1, 600))
        }).ToList();
        db.Customers.AddRange(customers);

        var statuses = Enum.GetValues<OrderStatus>();
        foreach (var c in customers.Take(32))            // last customers never order
        {
            var orderCount = rnd.Next(1, 7);
            for (int o = 0; o < orderCount; o++)
            {
                var order = new Order
                {
                    Customer = c,
                    OrderDate = new DateTime(2025, 1, 1).AddDays(rnd.Next(0, 700)),
                    Status = statuses[rnd.Next(statuses.Length)]
                };
                foreach (var p in products.OrderBy(_ => rnd.Next()).Take(rnd.Next(1, 5)))
                    order.Items.Add(new OrderItem
                    {
                        Product = p,
                        Quantity = rnd.Next(1, 5),
                        UnitPrice = p.Price,
                        Discount = rnd.Next(0, 3) * 0.05m
                    });
                db.Orders.Add(order);
            }
        }

        foreach (var p in products)
            foreach (var c in customers.OrderBy(_ => rnd.Next()).Take(rnd.Next(0, 15)))
                db.Reviews.Add(new Review
                {
                    Product = p,
                    Customer = c,
                    Rating = rnd.Next(3, 6),
                    Comment = "Seeded review",
                    CreatedAt = now.AddDays(-rnd.Next(1, 300))
                });

        db.SaveChanges();
    }
}
