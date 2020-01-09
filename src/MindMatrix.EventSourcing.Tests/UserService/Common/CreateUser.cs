namespace MindMatrix.EventSourcing.Tests
{
    using MediatR;

    public class CreateUser : IRequest<User>
    {
        public string Username { get; }
        public string Email { get; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public CreateUser(string username, string email)
        {
            Username = username;
            Email = email;
        }
    }

    public class CreatedUser : INotification
    {
        public string Id { get; }
        public CreateUser CreateUser { get; }

        public CreatedUser(string id, CreateUser user)
        {
            Id = id;
            CreateUser = user;
        }
    }
}