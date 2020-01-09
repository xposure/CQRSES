namespace MindMatrix.EventSourcing.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using MindMatrix.EventSourcing;
    using Shouldly;

    public class CreateUserHandler : IRequestHandler<CreateUser, User>
    {

        private readonly IMediator mediator;
        private readonly IEventStreamFactory<User> eventStreamFactory;

        public CreateUserHandler(IMediator mediator, IEventStreamFactory<User> eventStreamFactory)
        {
            this.mediator = mediator;
            this.eventStreamFactory = eventStreamFactory;
        }

        public async Task<User> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            //create in DB first
            var user = new User();

            var eventStream = eventStreamFactory.Create(string.Empty);

            eventStream.ShouldNotBeNull();
            eventStream.Version.ShouldBe(-1);


            eventStream.Append(new CreatedUser(string.Empty, request));


            await mediator.Publish(new CreatedUser(string.Empty, request));

            return user;
        }
    }

    public class CreatedUserHandler : INotificationHandler<CreatedUser>
    {
        private readonly IMediator mediator;

        public CreatedUserHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task Handle(CreatedUser notification, CancellationToken cancellationToken)
        {
            var user = new User();

            user.Username = notification.CreateUser.Username;
            user.Email = notification.CreateUser.Email;
            user.FirstName = notification.CreateUser.FirstName;
            user.LastName = notification.CreateUser.LastName;
            user.Deleted = false;
            return Task.CompletedTask;
        }
    }
}