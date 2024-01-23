namespace RestaurantApi3.Dtos;

public class RestaurantQuery
{
    public string? SearchBy { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    
    public string? SortBy { get; set; }

    public SortOrder? SortOrder { get; set; }


}

public enum SortOrder
{
    ASC, DESC
}