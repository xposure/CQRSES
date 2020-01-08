namespace MindMatrix.EventSourcing
{
    using System;
    using EventStore.ClientAPI;
    using MediatR;
    using StructureMap;
    using StructureMap.Building;
    using StructureMap.Pipeline;

    public class AggregateEventInstanceFactory : Instance
    {
        public override string Description => "Creates strongly typed AggregateEvents.";

        public override Type ReturnedType => typeof(AggregateEvent<,>);

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotSupportedException();
        }

        public override Instance CloseType(Type[] types)
        {
            // StructureMap will cache the object built out of this,
            // so the expensive Reflection hit only happens
            // once

            var instanceType = typeof(AggregateEvent<,>).MakeGenericType(types);
            return new ObjectInstance(Activator.CreateInstance(instanceType)).Named(types[1].FullName);
            //return ;//.As<Instance>();
        }
    }

    public class DIFixture
    {
        public static IContainer Scope() => _container.GetNestedContainer();
        public readonly static Container _container;

        //public readonly static Mediator Mediator;
        static DIFixture()
        {
            _container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<DIFixture>();
                    scanner.ConnectImplementationsToTypesClosing(typeof(IEvent<>));

                    //scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    //scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));

                    //scanner.ConnectImplementationsToTypesClosing(typeof(IAggregateEvent<>));
                });

                //cfg.For(typeof(IAggregateEventFactory<>)).Use(typeof(AggregateEventFactory<>)).Singleton();

                //cfg.For<IMongoClient>().Use(new MongoClient(new MongoUrl("mongodb://localhost:27017"))).Singleton();
                cfg.For<IEventStoreConnection>().Use(EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"))).Singleton();


                cfg.For(typeof(IEventStreamFactory<>)).Use(typeof(EventStreamFactory<>)).Singleton();
                //cfg.For<IEventStoreConnection>().Use(EventStoreConnection.Create("tcp://user:password@myserver:11234")).Singleton();

                //cfg.For(typeof(IEventSourceRepository<>)).Use(typeof(MongoESRespository<>));


                //cfg.For(typeof(IEventSourceRepository<>)).Use(typeof(InMemoryEventSourceRepository<>));

                //cfg.For(typeof(IAggregateStream<>)).Use(typeof(InMemoryAggregrateEvents<>));
                //cfg.For(typeof(IRepository<>)).Use(typeof(InMemoryRepository<>));
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>();

            });
            var connection = _container.GetInstance<IEventStoreConnection>();
            connection.ConnectAsync().Wait();


            //Mediator = Container.GetInstance<Mediator>();
        }
    }
}