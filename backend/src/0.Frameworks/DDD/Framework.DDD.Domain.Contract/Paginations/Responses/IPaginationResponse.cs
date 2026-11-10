namespace Framework.DDD.Domain.Contract.Paginations.Responses;

public interface IPaginationResponse<TResult>
{
    List<TResult> Items { get; set; }
    BasePaginationResult Base { get; set; }
}

public interface IPaginationResponse<TResult, TAdditional> : IPaginationResponse<TResult>
{
    TAdditional Additional { get; set; }
}
