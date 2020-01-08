namespace MindMatrix.EventSourcing
{
    using Xunit;
    using Shouldly;
    using System;
    using EventStore.ClientAPI;
    using System.Threading.Tasks;
    using StructureMap;
    using System.Collections.Generic;

    public interface IEventStreamFactory<TAggregate>
    {
        IEventStream<TAggregate> Create(string aggregateId);
    }

    public class EventStreamFactory<TAggregate> : IEventStreamFactory<TAggregate>
    {
        private readonly IEventStoreConnection _eventStore;
        public EventStreamFactory(IEventStoreConnection eventStore)
        {
            _eventStore = eventStore;
        }

        public IEventStream<TAggregate> Create(string aggregateId)
        {
            return new EventStream<TAggregate>(_eventStore, aggregateId);
        }
    }

    public interface IAggregateEvent<TAggregate> : IEvent<TAggregate>
    {
        string Id { get; }
        long Version { get; }

        IEvent<TAggregate> Data { get; }
    }

    public class AggregateEvent<TAggregate> : IAggregateEvent<TAggregate>
    {
        public string Id { get; }
        public long Version { get; }
        public IEvent<TAggregate> Data { get; }

        public AggregateEvent(string id, long version, IEvent<TAggregate> data)
        {
            Id = id;
            Version = version;
            Data = data;
        }

        public void Apply(TAggregate aggregate)
        {
            Data.Apply(aggregate);
        }
    }

    public interface IEvent<TAggregate>
    {

        void Apply(TAggregate aggregate);

    }

    public interface IEventStream<TAggregate>
    {
        long Version { get; }

        Task Append<TEvent>(TEvent data);
        IAsyncEnumerable<IAggregateEvent<TAggregate>> Read(int start = 0);
    }


    public class EventStream<TAggregate> : IEventStream<TAggregate>
    {
        private readonly IEventStoreConnection _eventStore;
        private readonly string _aggregateId;

        private long _version = -1;

        public long Version => _version;

        public EventStream(IEventStoreConnection eventStore, string aggregateId)
        {
            _eventStore = eventStore;
            _aggregateId = aggregateId;
        }

        public async Task Append<TEvent>(TEvent data)
        {
            var bytes = Json.ToJsonBytes(data);
            var result = await _eventStore.AppendToStreamAsync(_aggregateId, _version, new EventData(Guid.NewGuid(), data.GetType().FullName, true, bytes, null));
            _version = result.NextExpectedVersion;
        }

        public async IAsyncEnumerable<IAggregateEvent<TAggregate>> Read(int start = 0)
        {
            await foreach (var it in _eventStore.ReadEventsAsync(_aggregateId, start))
            {
                var type = Type.GetType(it.Event.EventType);
                var obj = (IEvent<TAggregate>)Json.ParseJson(it.Event.Data, type);

                yield return new AggregateEvent<TAggregate>(it.Event.EventId.ToString(), it.Event.EventNumber, obj);
            }
        }
    }

    public class Dummy
    {

    }

    public class DummyEvent : IEvent<Dummy>
    {
        public string Name { get; set; }
        public void Apply(Dummy aggregate)
        {
            throw new NotImplementedException();
        }
    }

    public class EventStreamTests
    {
        [Fact]
        public async void CanAppendToStream()
        {
            using var di = DIFixture.Scope();
            var factory = di.GetInstance<IEventStreamFactory<Dummy>>();
            var stream = factory.Create(Guid.NewGuid().ToString());

            await stream.Append(new DummyEvent() { Name = "Hello World" });

            //stream.Read

            stream.Version.ShouldBe(0);
        }

        [Fact]
        public async void CanReadFromStream()
        {
            var aggregateId = Guid.NewGuid().ToString();
            using var di = DIFixture.Scope();
            var factory = di.GetInstance<IEventStreamFactory<Dummy>>();
            var stream = factory.Create(aggregateId);

            await stream.Append(new DummyEvent() { Name = "Hello World" });

            var stream2 = factory.Create(aggregateId);
            var result = await stream2.Read().ToListAsync();

            result.Count.ShouldBe(1);
            result[0].ShouldBeOfType<AggregateEvent<Dummy>>().Version.ShouldBe(0);
            result[0].Data.ShouldBeOfType<DummyEvent>().Name.ShouldBe("Hello World");


            //stream.Read

            stream.Version.ShouldBe(0);
        }
    }
}