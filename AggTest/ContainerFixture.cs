namespace AggTest
{
    using AggCommon;
    using AggService.Customer;
    using MediatR;
    using StructureMap;

    public class ContainerFixture
    {
        public readonly static Container Container;

        public readonly static Mediator Mediator;
        static ContainerFixture()
        {
            Container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<CreateCustomer>(); // Our assembly with requests & handlers                    
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });

                cfg.For(typeof(IAggregrateEvents<>)).Use(typeof(InMemoryAggregrateEvents<>));
                cfg.For(typeof(IAggregrateRepository<>)).Use(typeof(InMemoryAggregrateRepository<>));
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>();
            });

            Mediator = Container.GetInstance<Mediator>();
        }
    }
}