namespace AggTest
{
    using Xunit;
    using Shouldly;
    using System;
    using AggService.Customer;
    using System.Threading.Tasks;
    using AggService;

    public class AggregrateTest
    {

        [Fact]
        public async void ShouldCreateCustomer()
        {
            var customerName = "John Doe";

            var mediator = ContainerFixture.Mediator;
            var command = new CreateCustomer() { Name = customerName };

            var result = await ContainerFixture.Mediator.Send(command);

            result.ShouldNotBeNull();
            result.Root.Name.ShouldBe(customerName);
        }
    }
}