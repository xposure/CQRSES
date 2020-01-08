namespace AggTest
{
    using System;
    using AggCommon;
    using AggRepo;
    using AggService.Customer;
    using EventStore.ClientAPI;
    using MediatR;
    using MindMatrix.EventSourcing;
    using MongoDB.Driver;
    using StructureMap;

    public class AggregateEventFactory<TAggregate> : IAggregateEventFactory<TAggregate>
    {
        private readonly IContainer _container;
        public AggregateEventFactory(IContainer container)
        {
            _container = container;
        }

        public AggregateEvent<TAggregate> Create(string type)
        {
            return _container.GetInstance<AggregateEvent<TAggregate>>(type);
        }
    }

    public class ContainerFixture
    {
        public static IContainer Container => _container.GetNestedContainer();
        public readonly static Container _container;

        //public readonly static Mediator Mediator;
        static ContainerFixture()
        {
            _container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<CreateCustomer>(); // Our assembly with requests & handlers                    
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));

                    scanner.ConnectImplementationsToTypesClosing(typeof(IAggregateEvent<>));
                });

                //var connection = new MongoClient(new MongoUrl("mongodb://localhost:27017"));

                cfg.For(typeof(IAggregateEventFactory<>)).Use(typeof(AggregateEventFactory<>)).Singleton();
                cfg.For(typeof(IAggregateEventFactory<>)).Use(typeof(AggregateEventFactory<>)).Singleton();

                cfg.For<IMongoClient>().Use(new MongoClient(new MongoUrl("mongodb://localhost:27017"))).Singleton();
                cfg.For<IEventStoreConnection>().Use(EventStoreConnection.Create(new Uri("tcp://admin:changeme@localhost:11234"))).Singleton();
                //cfg.For<IEventStoreConnection>().Use(EventStoreConnection.Create("tcp://user:password@myserver:11234")).Singleton();

                cfg.For(typeof(IEventSourceRepository<>)).Use(typeof(MongoESRespository<>));


                //cfg.For(typeof(IEventSourceRepository<>)).Use(typeof(InMemoryEventSourceRepository<>));

                cfg.For(typeof(IAggregateStream<>)).Use(typeof(InMemoryAggregrateEvents<>));
                cfg.For(typeof(IRepository<>)).Use(typeof(InMemoryRepository<>));
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>();

            });
            var connection = _container.GetInstance<IEventStoreConnection>();
            connection.ConnectAsync().Wait();
            Console.WriteLine(connection.ConnectionName);


            //Mediator = Container.GetInstance<Mediator>();
        }
    }
}