using Framework.DDD.Domain.Contract.Paginations.Enums;

namespace Framework.DDD.Domain.Contract.Paginations.Requests;

public interface IPaginationRequest
{
    RequestPaging Paging { get; set; }
    Dictionary<string, RequestOrderType> Orders { get; set; }
}

public interface IPaginationRequest<TFilter> : IPaginationRequest where TFilter : new()
{
    TFilter Filter { get; set; }
}
