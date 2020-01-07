namespace AggTest
{
    using Xunit;
    using Shouldly;
    using System;
    using AggService.Customer;
    using System.Threading.Tasks;
    using AggService;
    using MediatR;

    public class AggregrateTest
    {

        [Fact]
        public async void ShouldCreateCustomer()
        {
            var customerName = "John Doe";

            using var container = ContainerFixture.Container;
            var mediator = container.GetInstance<Mediator>(); ;
            var command = new CreateCustomer() { Name = customerName };

            var result = await mediator.Send(command);

            result.ShouldNotBeNull();
            result.Root.Name.ShouldBe(customerName);
        }

        [Fact]
        public async void ShouldThrowDuplicateCustomer()
        {
            var customerName = "John Doe";

            using var container = ContainerFixture.Container;
            var mediator = container.GetInstance<Mediator>(); ;
            var command = new CreateCustomer() { Name = customerName };

            var result = await mediator.Send(command);

            Should.Throw<CreateCustomer.CustomerAlreadyExists>(async () => await mediator.Send(command));
        }

        [Fact]
        public async void ShouldThrowOptimisticCheckException()
        {
            var customerName = "John Doe";

            using var container = ContainerFixture.Container;
            var mediator = container.GetInstance<Mediator>(); ;
            var command = new CreateCustomer() { Name = customerName };

            var result = await mediator.Send(command);

            Should.Throw<CreateCustomer.CustomerAlreadyExists>(async () => await mediator.Send(command));
        }
    }
}