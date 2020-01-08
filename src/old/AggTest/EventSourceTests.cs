namespace MindMatrix.EventSourcing
{
    using Xunit;
    using Shouldly;
    using System;
    using AggTest;

    public class DummyEvent : IEvent<Dummy>
    {
        public string Name { get; set; }

        public void Apply(Dummy dummy)
        {
            dummy.Name = Name;
        }
    }

    public class Dummy
    {
        public string Name { get; internal set; }
    }

    public class EventSourceTests
    {


        [Fact]
        public async void ShouldCreateNewValidAggregate()
        {
            var _eventSourceRepository = ContainerFixture.Container.GetInstance<IEventSourceRepository<Dummy>>();

            var dummy = await _eventSourceRepository.CreateAggregate();

            dummy.ShouldNotBeNull();
            dummy.Id.ShouldNotBeNull();
            dummy.Version.ShouldBe(0);

            var result = await _eventSourceRepository.GetAggregate(dummy.Id);
            result.ShouldNotBeNull();
            result.Id.ShouldBe(dummy.Id);
            result.Version.ShouldBe(dummy.Version);
        }

        [Fact]
        public async void ShouldAppendEventToNewAggregate()
        {
            var _eventSourceRepository = ContainerFixture.Container.GetInstance<IEventSourceRepository<Dummy>>();

            var dummy = await _eventSourceRepository.CreateAggregate();
            await dummy.Append(new DummyEvent() { Name = "hello world" });

            dummy.ShouldNotBeNull();
            dummy.Id.ShouldNotBeNull();
            dummy.Version.ShouldBe(1);
        }

        [Fact]
        public async void ShouldThrowExceptionOnModified()
        {
            var _eventSourceRepository = ContainerFixture.Container.GetInstance<IEventSourceRepository<Dummy>>();

            var dummy = await _eventSourceRepository.CreateAggregate();

            var stale = await _eventSourceRepository.GetAggregate(dummy.Id);

            await dummy.Append(new DummyEvent() { Name = "hello world" });

            Should.Throw<OptimisticConcurrencyCheckException>(async () =>
                        await stale.Append(new DummyEvent() { Name = "Gross" }));

        }
    }
}