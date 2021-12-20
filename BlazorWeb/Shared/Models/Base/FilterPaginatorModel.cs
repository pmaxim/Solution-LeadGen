namespace BlazorWeb.Shared.Models.Base
{
    public abstract class FilterPaginatorModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
