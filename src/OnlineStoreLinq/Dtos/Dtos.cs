namespace OnlineStoreLinq;

public record OrderTotalDto(int OrderId, string CustomerName, decimal Total);
public record CancelledDto(string FullName, int CancelledCount);
public record MonthlyReportDto(int Year, int Month, int OrderCount, decimal Revenue);
public record CategoryRevenueDto(string Category, decimal Revenue);
public record CityCountDto(string City, int Count);
public record CustomerSpendDto(string FullName, decimal Spent);
public record CustomerOrderCountDto(string FullName, int OrderCount);
public record ProductPairDto(string ProductA, string ProductB, int Times);
public record RankDto(int Rank, string Product, decimal Revenue);

public enum ProductSort { Name, Price, Rating }

public class ProductFilter
{
    public string Text { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool InStockOnly { get; set; }
    public ProductSort SortBy { get; set; } = ProductSort.Name;
    public bool Descending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public record ProductListItem(int Id, string Name, string Category, decimal Price, int Stock, double? AvgRating);
public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);
