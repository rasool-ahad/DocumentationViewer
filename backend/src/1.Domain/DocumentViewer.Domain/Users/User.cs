using Framework.DDD.Domain.Guards;
using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Framework.Guard.GuardClauses;

namespace DocumentViewer.Domain.Users;

public sealed class User : BaseUser<int>
{
    private User() { } // EF Core

    public User(string userName, string firstName, string lastName)
    {
        DomainGuard.ThrowIf.NotEmpty(userName, "A user name is required.");
        UserName = userName.Trim();
        ChangeName(firstName, lastName);
        IsActive = true;
    }

    public void ChangeName(string firstName, string lastName)
    {
        DomainGuard.ThrowIf.NotEmpty(firstName, "A first name is required.");
        DomainGuard.ThrowIf.NotEmpty(lastName, "A last name is required.");

        firstName = firstName.Trim();
        lastName = lastName.Trim();
        DomainGuard.ThrowIf.LessThanOrEqual(firstName.Length, 128, "First name cannot exceed 128 characters.");
        DomainGuard.ThrowIf.LessThanOrEqual(lastName.Length, 128, "Last name cannot exceed 128 characters.");

        FirstName = firstName;
        LastName = lastName;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void Block() => Blocked = true;

    public void Unblock() => Blocked = false;

    public void RecordLogin(DateTimeOffset occurredOnUtc) => LastLoggedOnUtc = occurredOnUtc.ToUniversalTime();
}

