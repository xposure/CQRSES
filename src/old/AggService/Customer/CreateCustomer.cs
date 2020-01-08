namespace AggService.Customer
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AggCommon;
    using AggRepo;
    using MediatR;

    public class CreateCustomer : IRequest<IAggregate<Customer>>
    {
        public class CustomerAlreadyExists : Exception
        {
            public CustomerAlreadyExists(string name) : base($"Customer [{name}] already exists.") { }
        }

        public string Name;

        public class CreatedCustomer : IEventState<Customer>
        {
            public readonly string Name;

            public CreatedCustomer(string name)
            {
                Name = name;
            }

            public void Apply(Customer aggregrate)
            {
                aggregrate.Name = Name;
            }
        }

        public class Handler : IRequestHandler<CreateCustomer, IAggregate<Customer>>
        {
            IRepository<Customer> _customers;
            //IAggregateRepository<Customer> _aggregates;
            //IAggregateStream<CustomerAggregrate> _events;

            public Handler(IRepository<Customer> customers)//, IAggregateRepository<Customer> aggregates)
            {
                _customers = customers;
                //_aggregates = aggregates;
                //_events = events;
            }

            public async Task<IAggregate<Customer>> Handle(CreateCustomer request, CancellationToken cancellationToken)
            {
                var aggregrateId = Guid.NewGuid().ToString();

                if (await _customers.Find(it => it.Root.Name == request.Name).AnyAsync(cancellationToken))
                    throw new CustomerAlreadyExists(request.Name);

                var customer = new Customer();
                customer.Name = request.Name;

                var aggregate = await _customers.AddAsync(customer);
                var eventData = new CreatedCustomer(request.Name);
                //var ev = await _events.Append(aggregate, eventData);
                await aggregate.Append(eventData);
                //ev.Apply();
                //aggregate.Apply(ev);

                return aggregate;
            }
        }
    }

}