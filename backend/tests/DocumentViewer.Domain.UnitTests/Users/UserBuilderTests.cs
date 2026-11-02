using DocumentViewer.Domain.Users;
using Framework.DDD.Domain.Exceptions;
using Xunit;

namespace DocumentViewer.Domain.UnitTests.Users;

public class UserBuilderTests
{
    [Fact]
    public void Build_creates_an_active_user_with_trimmed_fields()
    {
        var user = User.CreateBuilder()
            .WithUserName("  alice  ")
            .WithFirstName("  Alice  ")
            .WithLastName("  Smith  ")
            .Build();

        Assert.Equal("alice", user.UserName);
        Assert.Equal("Alice", user.FirstName);
        Assert.Equal("Smith", user.LastName);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Build_rejects_missing_required_fields()
    {
        Assert.Throws<DomainException>(() => User.CreateBuilder().Build());
        Assert.Throws<DomainException>(() => User.CreateBuilder()
            .WithUserName("alice")
            .WithFirstName("Alice")
            .Build());
    }
}
