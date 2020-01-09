namespace MindMatrix.EventSourcing.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public partial class User
    {

        public string Username { get; internal set; }
        public string Email { get; internal set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }

        public bool Deleted { get; internal set; }

        public class DeleteUser
        {
            public string Username { get; }

            public void Apply(User user)
            {
                user.Deleted = true;
            }
        }
    }

    public interface IUserService
    {
        Task<User> CreateUser(string username);
        Task<bool> DeletUser(string username);
        Task<bool> UpdateUser(string username);
    }
}