namespace MindMatrix.EventSourcing
{
    using Xunit;
    using Shouldly;
    using System;
    using EventStore.ClientAPI;
    using System.Threading.Tasks;
    using StructureMap;

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


    public interface IAggregateEventFactory<TAggregate>
    {
        IAggregateEvent<TAggregate> Create<TData>() where TData : new();
    }

    public class AggregateEventFactory<TAggregate> : IAggregateEventFactory<TAggregate>
    {
        public IAggregateEvent<TAggregate> Create<TData>()
            where TData : new()
        {
            return new AggregateEvent<TAggregate, TData>();
        }
    }

    public interface IAggregateEventFactory<TAggregate, TData>
        where TData : new()
    {
        IAggregateEvent<TAggregate> Create();
    }

    public class AggregateEventFactory<TAggregate, TData> : IAggregateEventFactory<TAggregate, TData>
        where TData : new()
    {
        public IAggregateEvent<TAggregate> Create()
        {
            return new AggregateEvent<TAggregate, TData>();
        }
    }


    public interface IAggregateEvent<TAggregate> : IEvent<TAggregate>
    {
        string Id { get; }
        long Index { get; }

    }

    public interface IAggregateEvent<TAggregate, TData> : IAggregateEvent<TAggregate>
    {
        TData Data { get; }
    }

    public class AggregateEvent<TAggregate, TData> : IAggregateEvent<TAggregate, TData>
    {
        public string Id => throw new NotImplementedException();

        public long Index => throw new NotImplementedException();

        public TData Data => throw new NotImplementedException();

        public void Apply(TAggregate aggregate)
        {
            throw new NotImplementedException();
        }
    }

    public interface IEvent<TAggregate>
    {

        void Apply(TAggregate aggregate);

    }

    // public class Event<TAggregate> : IEvent<TAggregate>
    // {

    // }

    public interface IEventFactory<TAggregate>
    {
        IEvent<TAggregate> Create(string type);
    }

    public class EventFactory<TAggregate> : IEventFactory<TAggregate>
    {
        private readonly IContainer _container;
        public EventFactory(IContainer container)
        {
            _container = container;
        }

        public IEvent<TAggregate> Create(string type)
        {
            return _container.GetInstance<IEvent<TAggregate>>(type);
        }
    }

    public interface IEventStream<TAggregate>
    {
        long Version { get; }

        Task Append<TEvent>(TEvent data);
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
    }

    public class Dummy
    {

    }

    public class DummyEvent : IEvent<Dummy>
    {
        public void Apply(Dummy aggregate)
        {
            throw new NotImplementedException();
        }
    }

    public class EventStreamTests
    {
        [Fact]
        public async void HelloWorld()
        {
            using var di = DIFixture.Scope();
            var factory = di.GetInstance<IEventStreamFactory<Dummy>>();
            var stream = factory.Create(Guid.NewGuid().ToString());

            await stream.Append(new DummyEvent());

            stream.Version.ShouldBe(0);
        }
    }
}