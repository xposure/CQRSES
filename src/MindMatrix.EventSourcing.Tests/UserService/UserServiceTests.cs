namespace MindMatrix.EventSourcing.Tests
{
    using Shouldly;
    using Xunit;
    using System;
    using MediatR;
    using StructureMap;
    using MindMatrix.EventSourcing;
    using System.Collections.Generic;

    public class UserServiceTests
    {
        private Container _container;

        public UserServiceTests()
        {
            _container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<UserServiceTests>();
                    //scanner.ConnectImplementationsToTypesClosing(typeof(INotification));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });

                cfg.For(typeof(IJsonSerializer<>)).Use(typeof(JsonSerializer<>)).Singleton();
                cfg.For<IJsonSerializerFactory>().Use<JsonSerializerFactory>().Singleton();
                cfg.For(typeof(IEventStreamFactory<>)).Use(typeof(MemoryEventStreamFactory<>));
                cfg.For(typeof(IAggregateEventFactory<,>)).Use(typeof(AggregateEventFactory<,>)).Singleton();
                cfg.For(typeof(IAggregateEventFactoryGenerator<>)).Use(typeof(AggregateEventFactoryGenerator<>));
                cfg.For(typeof(IAggregateRepository<>)).Use(typeof(MemoryAggregateRepository<>));
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>();
            });
        }


        public class JsonSerializerFactory : IJsonSerializerFactory
        {
            private readonly IContainer _container;

            public JsonSerializerFactory(IContainer container)
            {
                _container = container;
            }

            public IJsonSerializer GetSerializer(Type type)
            {
                var serializerType = typeof(JsonSerializer<>).MakeGenericType(type);
                return (IJsonSerializer)_container.GetInstance(serializerType);
            }

            public IJsonSerializer<T> GetSerializer<T>() => _container.GetInstance<IJsonSerializer<T>>();
        }



        public class AggregateEventFactory<TAggregate, TEvent> : IAggregateEventFactory<TAggregate, TEvent>
        {
            private class AggregateEvent : IAggregateEvent<TAggregate, TEvent>
            {
                public string Id { get; set; }

                public long Version { get; set; }

                public IAggregate<TAggregate> Aggregate { get; set; }

                public TEvent Data { get; set; }
            }

            private readonly IJsonSerializer<TEvent> _eventSerializer;
            public AggregateEventFactory(IJsonSerializerFactory _serializerFactory)
            {
                _eventSerializer = _serializerFactory.GetSerializer<TEvent>();
            }

            public IAggregateEvent Create(IAggregate<TAggregate> aggregate, IAggregateStreamEvent ev)
            {
                var e = _eventSerializer.ReadT(ev.Bytes);
                return new AggregateEvent() { Aggregate = aggregate, Id = ev.Id, Version = ev.Version, Data = e };
            }
        }

        public class AggregateEventFactoryGenerator<TAggregate> : IAggregateEventFactoryGenerator<TAggregate>
        {
            private readonly IContainer _container;
            private Dictionary<Type, IAggregateEventFactory<TAggregate>> _eventFactories = new Dictionary<Type, IAggregateEventFactory<TAggregate>>();

            public AggregateEventFactoryGenerator(IContainer container)
            {
                _container = container;
            }

            public IAggregateEventFactory<TAggregate> GetFactory(Type type)
            {
                if (!_eventFactories.TryGetValue(type, out var factory))
                {
                    var serializerType = typeof(IAggregateEventFactory<,>).MakeGenericType(new[] { typeof(TAggregate), type });
                    factory = (IAggregateEventFactory<TAggregate>)_container.GetInstance(serializerType);
                    _eventFactories.Add(type, factory);
                }

                return factory;
            }
        }

        [Fact]
        public async void ShouldCreateUser()
        {
            var di = _container.GetNestedContainer();
            // var serializerFactory = di.GetInstance<IJsonSerializerFactory>();
            // var writeSerializer = serializerFactory.GetSerializer<CreatedUser>();
            // var readSerializer = serializerFactory.GetSerializer(typeof(CreatedUser));

            // var cu = new CreatedUser("123", null);
            // var bytes = writeSerializer.Write(cu);
            // var cu2 = readSerializer.Read(bytes);
            // //Type.GetType()
            // cu.Id.ShouldBe(cu2.ShouldBeOfType<CreatedUser>().Id);

            //throw new Exception(di.WhatDoIHave());

            // var x = Json.ParseJsonT<CreatedUser>("{\"id\": \"asdf\",\"createUser\": {\"username\": \"john.doe\",\"email\": \"john.doe@gmail.com\"}}");
            // x.ShouldNotBeNull();
            // x.CreateUser.ShouldNotBeNull();
            var username = "john.doe";
            var email = username + "@gmail.com";

            var m = di.GetInstance<IMediator>();
            var user = await m.Send(new CreateUser(username, email));

            user.Root.Username.ShouldBe(username);
            user.Root.Email.ShouldBe(email);

        }
    }
}