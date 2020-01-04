namespace AggService.Customer
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AggCommon;
    using MediatR;
    using MediatR.Pipeline;

    public class CreateCustomer : IRequest<IAggregrate<CustomerAggregrate>>
    {
        public class CustomerAlreadyExists : Exception
        {
            public CustomerAlreadyExists(string name) : base($"Customer [{name}] already exists.") { }
        }

        public string Name;

        public class CreatedCustomer : IEventState<CustomerAggregrate>
        {
            public readonly string Name;

            public CreatedCustomer(string name)
            {
                Name = name;
            }

            public void Apply(CustomerAggregrate aggregrate)
            {
                aggregrate.Name = Name;
            }
        }

        public class Handler : IRequestHandler<CreateCustomer, IAggregrate<CustomerAggregrate>>
        {
            IAggregrateRepository<CustomerAggregrate> _repository;
            IAggregrateEvents<CustomerAggregrate> _events;

            public Handler(IAggregrateRepository<CustomerAggregrate> customers, IAggregrateEvents<CustomerAggregrate> events)
            {
                _repository = customers;
                _events = events;
            }

            public async Task<IAggregrate<CustomerAggregrate>> Handle(CreateCustomer request, CancellationToken cancellationToken)
            {
                var aggregrateId = Guid.NewGuid().ToString();

                if (await _repository.Find(it => it.Root.Name == request.Name).AnyAsync(cancellationToken))
                    throw new CustomerAlreadyExists(request.Name);

                var customer = new CustomerAggregrate();
                if (customer == null)
                    throw new InvalidOperationException();

                customer = new CustomerAggregrate();
                var aggregate = await _repository.AddAsync(customer);

                var eventData = new CreatedCustomer(request.Name);
                var ev = await _events.Append(aggregate, eventData);

                aggregate.Apply(ev);

                return aggregate;
            }
        }
    }

}