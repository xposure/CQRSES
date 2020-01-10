namespace MindMatrix.EventSourcing.Tests
{
    public partial class User
    {

        public string Username { get; internal set; }
        public string Email { get; internal set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }

        public bool Deleted { get; internal set; }

    }
}