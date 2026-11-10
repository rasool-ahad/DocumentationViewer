using Framework.DDD.Domain.Contract.Paginations.Enums;

namespace Framework.DDD.Domain.Contract.Paginations.Requests;

// ASP.NET Core binds query keys such as paging.index, paging.size, and filter.search
// to the nested properties when a controller parameter is marked [FromQuery].
public class PaginationRequest : IPaginationRequest
{
    public RequestPaging Paging { get; set; } = new();
    public Dictionary<string, RequestOrderType> Orders { get; set; } = [];
}

public class PaginationRequest<TFilter> : PaginationRequest, IPaginationRequest<TFilter>
    where TFilter : new()
{
    public TFilter Filter { get; set; } = new();
}
