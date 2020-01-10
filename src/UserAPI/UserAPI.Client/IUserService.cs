namespace UserAPI
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using MindMatrix.EventSourcing;

    public interface IUserService
    {
        Task<IAggregate<User>> Create(CreateUser user, CancellationToken cancellationToken = default);
    }

    public class MediatRUserService : IUserService
    {
        private readonly IMediator _mediator;

        public MediatRUserService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<IAggregate<User>> Create(CreateUser user, CancellationToken cancellationToken) => _mediator.Send(user, cancellationToken);
    }
}