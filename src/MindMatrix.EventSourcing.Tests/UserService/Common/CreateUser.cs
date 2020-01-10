namespace MindMatrix.EventSourcing.Tests
{
    using System.IO;
    using MediatR;

    public class CreateUser : IRequest<IAggregate<User>>
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
        public string Id { get; set; }
        public CreateUser Request { get; set; }

        public CreatedUser()
        {
        }
    }

}