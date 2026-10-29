using Microsoft.EntityFrameworkCore;

namespace OnlineStoreLinq;

/// <summary>
/// All the task solutions, split into partial files (one per level).
/// Line total = Quantity * UnitPrice * (1 - Discount). Revenue/spending = Delivered orders only.
/// </summary>
public partial class Solutions
{
    private readonly AppDbContext db;
    public Solutions(AppDbContext db) => this.db = db;
}
