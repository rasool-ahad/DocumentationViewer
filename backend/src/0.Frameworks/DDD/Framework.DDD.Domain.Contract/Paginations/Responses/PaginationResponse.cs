namespace Framework.DDD.Domain.Contract.Paginations.Responses;

public class PaginationResponse<TResult>(List<TResult> items, BasePaginationResult @base)
    : IPaginationResponse<TResult>
{
    public List<TResult> Items { get; set; } = items;
    public BasePaginationResult Base { get; set; } = @base;
}

public class PaginationResponse<TResult, TAdditional>(
    List<TResult> items, BasePaginationResult @base, TAdditional additional)
    : PaginationResponse<TResult>(items, @base), IPaginationResponse<TResult, TAdditional>
{
    public TAdditional Additional { get; set; } = additional;
}
