namespace Framework.DDD.Domain.Contract.Paginations.Requests;

public sealed class RequestPaging
{
    public int Index { get; set; }
    public int Size { get; set; } = 10;
    public int? Draw { get; set; }
}
