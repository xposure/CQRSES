namespace MindMatrix.EventSourcing.Tests
{
    using System.Threading.Tasks;

    public class User
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public bool Deleted { get; private set; }

        public class CreateUser
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

            public void Apply(User user)
            {
                user.Username = Username;
                user.Email = Email;
                user.FirstName = FirstName;
                user.LastName = LastName;
                user.Deleted = false;
            }
        }

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