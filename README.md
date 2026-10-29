# Online Store: LINQ Task (EF Core)

Solution for the 25-question LINQ task on an online-store database, plus the bonus questions.

## Requirements
- .NET 8 SDK
- SQL Server (LocalDB comes with Visual Studio) or any SQL Server instance

## Run
1. Open `OnlineStoreLinq.sln` in Visual Studio.
2. (Optional) change the connection string at the top of `src/OnlineStoreLinq/Program.cs`.
3. Press **F5**. The database is created and seeded automatically, then every query runs and prints a sample of its result.

CLI: `dotnet run --project src/OnlineStoreLinq`

## Structure
| Path | Content |
|------|---------|
| `Models/` | Entities (Customer, Order, OrderItem, Product, Category, Review) |
| `Data/` | `AppDbContext` and `DbSeeder` (fake data) |
| `Queries/Level1Queries.cs` | Q1-Q6, warm-up |
| `Queries/Level2Queries.cs` | Q7-Q14, grouping and aggregation |
| `Queries/Level3Queries.cs` | Q15-Q22, joins, subqueries, set logic |
| `Queries/Level4Queries.cs` | Q23-Q25, `SearchProducts`, `ExecuteUpdate`, `ExecuteDelete` |
| `Queries/BonusQueries.cs` | `ToQueryString()`, N+1 explanation, why `Select((x,i))` fails |
| `Dtos/` | Result records and `ProductFilter` |

## Notes
- Line total = `Quantity * UnitPrice * (1 - Discount)`.
- Revenue and spending use **Delivered** orders only.
- Q24 and Q25 modify data, so they run last in `Program.cs`.
