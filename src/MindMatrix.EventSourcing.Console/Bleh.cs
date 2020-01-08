// namespace MindMatrix.EventSourcing
// {
//     using System;
//     using EventStore.ClientAPI;
//     using MediatR;
//     using StructureMap;
//     using StructureMap.Building;
//     using StructureMap.Pipeline;



//     public class CustomInterception : IInstancePolicy
//     {

//         public void Apply(Type pluginType, Instance instance)
//         {
//             Console.WriteLine(pluginType);
//             Console.WriteLine(instance);
//         }
//     }

//     public class Bleh
//     {
//         public class AggregateEventInstance : Instance
//         {
//             public override string Description => "Creates strongly typed AggregateEvents.";

//             public override Type ReturnedType => typeof(AggregateEventFactory<,>);
//             private string _name;
//             //public override string Name => _name;
//             public AggregateEventInstance() { }
//             public AggregateEventInstance(IAggregateEventFactory factory)
//             {
//                 _name = name;
//                 Console.WriteLine(name);
//             }

//             public override IDependencySource ToDependencySource(Type pluginType)
//             {
//                 throw new NotSupportedException();
//             }


//             public override Instance CloseType(Type[] types)
//             {
//                 // StructureMap will cache the object built out of this,
//                 // so the expensive Reflection hit only happens
//                 // once

//                 Console.WriteLine(Name);
//                 Console.WriteLine(types[0].FullName);
//                 var instanceType = typeof(AggregateEventFactory<,>).MakeGenericType(types);
//                 return new ObjectInstance(Activator.CreateInstance(instanceType)).Named(types[1].FullName);
//                 //return ;//.As<Instance>();
//             }
//         }

//         public class AggregateEventInstanceFactory : Instance
//         {
//             public override string Description => "Creates strongly typed AggregateEvents.";

//             public override Type ReturnedType => typeof(AggregateEventFactory<,>);
//             private string _name;
//             //public override string Name => _name;
//             public AggregateEventInstanceFactory() { }
//             public AggregateEventInstanceFactory(string name)
//             {
//                 _name = name;
//                 Console.WriteLine(name);
//             }

//             public override IDependencySource ToDependencySource(Type pluginType)
//             {
//                 throw new NotSupportedException();
//             }


//             public override Instance CloseType(Type[] types)
//             {
//                 // StructureMap will cache the object built out of this,
//                 // so the expensive Reflection hit only happens
//                 // once

//                 Console.WriteLine(Name);
//                 Console.WriteLine(types[0].FullName);
//                 var instanceType = typeof(AggregateEventFactory<,>).MakeGenericType(types);
//                 return new ObjectInstance(Activator.CreateInstance(instanceType)).Named(types[1].FullName);
//                 //return ;//.As<Instance>();
//             }
//         }
//         //     public static IContainer Scope() => _container.GetNestedContainer();
//         //     public readonly static Container _container;

//         //public readonly static Mediator Mediator;
//         public static Container Create()
//         {
//             var _container = new Container(cfg =>
//             {
//                 cfg.Scan(scanner =>
//                 {
//                     scanner.AssemblyContainingType<DIFixture>();
//                     scanner.ConnectImplementationsToTypesClosing(typeof(IEvent<>));

//                     //scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
//                     //scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));

//                     //scanner.ConnectImplementationsToTypesClosing(typeof(IAggregateEvent<>));
//                 });

//                 cfg.Policies.Add(new CustomInterception());

//                 cfg.For(typeof(IAggregateEventFactory<>)).Use(typeof(AggregateEventFactory<>)).Singleton();
//                 cfg.For(typeof(IAggregateEvent<,>)).Use(new AggregateEventInstanceFactory());
//                 //cfg.For(typeof(IAggregateEvent<>)).Use(new AggregateEventInstanceFactory());

//                 //var connection = new MongoClient(new MongoUrl("mongodb://localhost:27017"));

//                 //cfg.For(typeof(IAggregateEventFactory<>)).Use(typeof(AggregateEventFactory<>)).Singleton();
//                 //cfg.For(typeof(IAggregateEventFactory<>)).Use(typeof(AggregateEventFactory<>)).Singleton();

//                 //cfg.For<IMongoClient>().Use(new MongoClient(new MongoUrl("mongodb://localhost:27017"))).Singleton();
//                 cfg.For<IEventStoreConnection>().Use(EventStoreConnection.Create(new Uri("tcp://admin:changeme@localhost:11234"))).Singleton();


//                 cfg.For(typeof(IEventStreamFactory<>)).Use(typeof(EventStreamFactory<>)).Singleton();
//                 //cfg.For<IEventStoreConnection>().Use(EventStoreConnection.Create("tcp://user:password@myserver:11234")).Singleton();

//                 //cfg.For(typeof(IEventSourceRepository<>)).Use(typeof(MongoESRespository<>));


//                 //cfg.For(typeof(IEventSourceRepository<>)).Use(typeof(InMemoryEventSourceRepository<>));

//                 //cfg.For(typeof(IAggregateStream<>)).Use(typeof(InMemoryAggregrateEvents<>));
//                 //cfg.For(typeof(IRepository<>)).Use(typeof(InMemoryRepository<>));
//                 cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
//                 cfg.For<IMediator>().Use<Mediator>();

//             });
//             //var connection = _container.GetInstance<IEventStoreConnection>();
//             //connection.ConnectAsync().Wait();

//             return _container;

//             //Mediator = Container.GetInstance<Mediator>();
//         }
//     }
// }