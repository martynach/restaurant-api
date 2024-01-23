namespace RestaurantApi3.Dtos;

public class PagedResult<TItem>
{
    public PagedResult(List<TItem> items, int totalItemsCount, int pageSize, int pageNumber)
    {
        Items = items;
        TotalItemsCount = totalItemsCount;
        TotalPages= (int)Math.Ceiling((double)totalItemsCount / pageSize);
        ItemsFrom = pageSize * (pageNumber - 1) + 1;
        ItemsTo = pageSize * pageNumber;
    }

    public List<TItem>Items { get;}
    public int TotalPages { get; }
    public int TotalItemsCount { get; }
    
    // range
    public int ItemsFrom { get;  }
    public int ItemsTo { get; }
    
     
}