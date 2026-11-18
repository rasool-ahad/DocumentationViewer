namespace DocumentViewer.Domain.Navigation;

// A domain service or application service implements cross-aggregate parent checks.
public interface INavigationTreeValidator
{
    Task ValidateParentAsync(long? parentId, long itemId, CancellationToken cancellationToken);
}
