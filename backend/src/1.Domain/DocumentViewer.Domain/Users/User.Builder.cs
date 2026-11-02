namespace DocumentViewer.Domain.Users;

public sealed partial class User
{
    public static Builder CreateBuilder() => new();

    public sealed class Builder
    {
        private string _userName = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;

        internal Builder() { }

        public Builder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public Builder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public Builder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public User Build()
        {
            var user = new User();
            user.ChangeUserName(_userName);
            user.ChangeName(_firstName, _lastName);
            user.Activate();
            return user;
        }
    }
}
