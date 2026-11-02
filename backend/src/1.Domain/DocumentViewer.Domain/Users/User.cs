using Framework.DDD.Domain.Toolkits.Entities.Identity;

namespace DocumentViewer.Domain.Users;

public sealed partial class User : BaseUser<int>
{
    private User() { } // EF Core and the builder
}

