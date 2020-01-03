namespace AggService.Customer
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AggCommon;
    using MediatR;

    public class CreateCustomer : IRequest<CustomerAggregrate>
    {
        public class CustomerAlreadyExists : Exception
        {
            public CustomerAlreadyExists(string name) : base($"Customer [{name}] already exists.") { }
        }

        public string Name;

        public class CreatedCustomer : AggregrateEvent<CustomerAggregrate>
        {
            public readonly string Name;

            public CreatedCustomer(string name)
            {
                Name = name;
            }

            protected override void OnApply(CustomerAggregrate aggregrate)
            {
                aggregrate.Name = Name;
            }
        }

        public class Handler : IRequestHandler<CreateCustomer, CustomerAggregrate>
        {
            IAggregrateRepository<CustomerAggregrate> _repository;
            IAggregrateEvents<CustomerAggregrate> _events;

            public Handler(IAggregrateRepository<CustomerAggregrate> customers, IAggregrateEvents<CustomerAggregrate> events)
            {
                _repository = customers;
                _events = events;
            }

            public async Task<CustomerAggregrate> Handle(CreateCustomer request, CancellationToken cancellationToken)
            {
                var aggregrateId = Guid.NewGuid().ToString();

                if (await _repository.Find(it => it.Name == request.Name).AnyAsync(cancellationToken))
                    throw new CustomerAlreadyExists(request.Name);


                var customer = new CustomerAggregrate(aggregrateId);
                if (customer == null)
                    throw new InvalidOperationException();

                var aggregrate = new CustomerAggregrate(aggregrateId);

                var ev = new CreatedCustomer(request.Name);

                //TODO: check if we appended to the stream
                await _events.Append(aggregrateId, ev);

                ev.Apply(aggregrate);

                //snapshotting ?
                await _repository.AddAsync(aggregrate);

                return aggregrate;
            }
        }
    }

}