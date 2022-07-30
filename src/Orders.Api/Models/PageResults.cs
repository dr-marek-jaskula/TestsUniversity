namespace Orders.Api.Models;

public class PageResult<T>
{
    public List<T> Items { get; set; }
    public int TotalItemsCount { get; set; }
    public int TotalPages { get; set; }

    //The first element of the certain page
    public int ItemsFrom { get; set; }

    //The last element of the certain page
    public int ItemsTo { get; set; }

    public PageResult(List<T> items, int totalCount, int pageSize, int pageNumber)
    {
        Items = items;
        TotalItemsCount = totalCount;

        ItemsFrom = pageSize * (pageNumber - 1) + 1;
        ItemsTo = ItemsFrom + pageSize - 1;

        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}