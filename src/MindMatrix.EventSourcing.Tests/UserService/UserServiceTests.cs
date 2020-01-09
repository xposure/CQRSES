namespace MindMatrix.EventSourcing.Tests
{
    using Shouldly;
    using Xunit;
    using System;
    using MediatR;
    using StructureMap;

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
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });

                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>();
            });
        }

        [Fact]
        public async void ShouldCreateUser()
        {
            var di = _container.GetNestedContainer();
            var username = "john.doe";
            var email = username + "@gmail.com";

            var m = di.GetInstance<IMediator>();
            var user = await m.Send(new CreateUser(username, email));

            user.Username.ShouldBe(username);
            user.Email.ShouldBe(email);



        }
    }
}