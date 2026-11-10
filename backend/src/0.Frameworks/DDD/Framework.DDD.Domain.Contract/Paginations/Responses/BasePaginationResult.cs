namespace Framework.DDD.Domain.Contract.Paginations.Responses;

public sealed class BasePaginationResult(long total, int index = 0, int size = 10)
{
    public long Total { get; set; } = total;
    public int Index { get; set; } = index;
    public int Size { get; set; } = size;
}
