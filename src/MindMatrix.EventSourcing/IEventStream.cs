namespace MindMatrix.EventSourcing
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EventStore.ClientAPI;
    using MediatR;

    public interface IEventStream<TAggregate>
    {
        long Version { get; }

        Task<IAggregateEvent<TAggregate, TEvent>> Append<TEvent>(TEvent data) where TEvent : IEvent<TAggregate>;
        IAsyncEnumerable<IAggregateEvent<TAggregate>> Read(int start = 0);
    }


    public class EventStream<TAggregate> : IEventStream<TAggregate>
    {
        private readonly IMediator _mediator;
        private readonly IEventStoreConnection _eventStore;
        private readonly string _aggregateId;

        private long _version = -1;

        public long Version => _version;

        public EventStream(IMediator mediator, IEventStoreConnection eventStore, string aggregateId)
        {
            _mediator = mediator;
            _eventStore = eventStore;
            _aggregateId = aggregateId;
        }

        public async Task<IAggregateEvent<TAggregate, TEvent>> Append<TEvent>(TEvent data)
            where TEvent : IEvent<TAggregate>
        {
            var type = typeof(TEvent);
            var eid = Guid.NewGuid();
            var version = _version;
            var bytes = Json.ToJsonBytes(data);

            var result = await _eventStore.AppendToStreamAsync(_aggregateId, _version, new EventData(eid, type.FullName, true, bytes, null));
            _version = result.NextExpectedVersion;

            await _mediator.Publish(data);

            return new AggregateEvent<TAggregate, TEvent>(eid.ToString(), version, data);
        }

        public async IAsyncEnumerable<IAggregateEvent<TAggregate>> Read(int start = 0)
        {
            await foreach (var it in _eventStore.ReadEventsAsync(_aggregateId, start))
            {
                var type = Type.GetType(it.Event.EventType, true);
                var obj = Json.ParseJson(it.Event.Data, type);

                yield return new AggregateEvent<TAggregate>(it.Event.EventId.ToString(), it.Event.EventNumber, (IEvent<TAggregate>)obj);
            }
        }
    }

    public class MemoryEventStream<TAggregate> : IEventStream<TAggregate>
    {
        private class EventData
        {
            public string Id { get; }
            public long Version { get; }
            public string EventType { get; }
            public byte[] Data { get; }
            public EventData(string id, long version, string type, byte[] data)
            {
                Id = id;
                Version = version;
                EventType = type;
                Data = data;
            }
        }

        private readonly string _aggregateId;

        private long _version = -1;

        public long Version => _version;

        private List<EventData> _events = new List<EventData>();

        public MemoryEventStream(string aggregateId)
        {
            _aggregateId = aggregateId;
        }

        public Task<IAggregateEvent<TAggregate, TEvent>> Append<TEvent>(TEvent data)
            where TEvent : IEvent<TAggregate>
        {
            var type = typeof(TEvent);
            var eid = Guid.NewGuid();
            var version = _version;
            var bytes = Json.ToJsonBytes(data);

            if (_events.Count - 1 != _version)
                throw new InvalidOperationException($"Optimistic Concurrency Check, expected: {_version}");

            _version++;
            _events.Add(new EventData(eid.ToString(), _version, type.FullName, bytes));
            return Task.FromResult<IAggregateEvent<TAggregate, TEvent>>(new AggregateEvent<TAggregate, TEvent>(eid.ToString(), version, data));
        }

        public async IAsyncEnumerable<IAggregateEvent<TAggregate>> Read(int start = 0)
        {
            await Task.CompletedTask;

            for (var i = start; i < _events.Count; i++)
            {
                var type = Type.GetType(_events[i].EventType);
                var obj = (IEvent<TAggregate>)Json.ParseJson(_events[i].Data, type);

                yield return new AggregateEvent<TAggregate>(_events[i].Id, _events[i].Version, obj);
            }
        }
    }
}