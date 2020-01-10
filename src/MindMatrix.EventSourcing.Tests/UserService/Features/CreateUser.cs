namespace MindMatrix.EventSourcing.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using MindMatrix.EventSourcing;
    using Shouldly;

    public class CreateUserHandler : IRequestHandler<CreateUser, IAggregate<User>>
    {
        private readonly IAggregateRepository<User> _repository;

        public CreateUserHandler(IMediator mediator, IAggregateRepository<User> repository)
        {
            _repository = repository;
        }

        public async Task<IAggregate<User>> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            //create in DB first
            var id = Guid.NewGuid().ToString();
            var user = await _repository.Get(id);
            await _repository.Append(user, new CreatedUser() { Id = id, Request = request });

            return user;
        }
    }

    public class CreatedUserHandler : INotificationHandler<IAggregateEvent<User, CreatedUser>>
    {
        private readonly IMediator mediator;

        public CreatedUserHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task Handle(IAggregateEvent<User, CreatedUser> notification, CancellationToken cancellationToken)
        {
            var user = notification.Aggregate.Root;
            var ev = notification.Data;
            notification.Data.Id.ShouldNotBeNullOrEmpty();
            user.Username = ev.Request.Username;
            user.Email = ev.Request.Email;
            user.FirstName = ev.Request.FirstName;
            user.LastName = ev.Request.LastName;
            user.Deleted = false;
            return Task.CompletedTask;
        }
    }
}