namespace MindMatrix.EventSourcing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using EventStore.ClientAPI;

    public class EventStoreEventStream<TAggregate> : IEventStream
    {
        private readonly IEventStoreConnection _eventStore;
        private readonly string _aggregateId;

        private long _version = -1;

        public long Version => _version;
        private readonly Assembly _assembly = typeof(TAggregate).Assembly;

        public EventStoreEventStream(IEventStoreConnection eventStore, string aggregateId)
        {
            _eventStore = eventStore;
            _aggregateId = aggregateId;
        }

        public async Task<IAggregateStreamEvent<TEvent>> Append<TEvent>(TEvent data)
        {
            var type = data.GetType();
            var eid = Guid.NewGuid();
            var version = _version;
            var bytes = Json.ToJsonBytes(data);

            var result = await _eventStore.AppendToStreamAsync(_aggregateId, _version, new EventData(eid, type.FullName, true, bytes, null));
            _version = result.NextExpectedVersion;

            return new AggregateStreamEvent<TEvent>(type, eid.ToString(), version, bytes, data);
        }

        public async IAsyncEnumerable<IAggregateStreamEvent> Read(int start = 0)
        {
            await foreach (var it in _eventStore.ReadEventsAsync(_aggregateId, start))
            {
                var type = _assembly.GetType(it.Event.EventType);
                yield return new AggregateStreamEvent(type, it.Event.EventId.ToString(), it.Event.EventNumber, it.Event.Data, it.Event.Metadata);
            }
        }
    }
}
