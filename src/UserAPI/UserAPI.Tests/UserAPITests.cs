namespace UserAPI
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
                    scanner.AssemblyContainingType<UserAPI.Worker>();
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
                cfg.For<IUserService>().Use<MediatRUserService>();
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

        [Fact]
        public async void ShouldCreateUser()
        {
            var di = _container.GetNestedContainer();
            var username = "john.doe";
            var email = username + "@gmail.com";

            var service = di.GetInstance<IUserService>();
            var user = await service.Create(new CreateUser(username, email));

            user.Root.Username.ShouldBe(username);
            user.Root.Email.ShouldBe(email);

        }
    }
}