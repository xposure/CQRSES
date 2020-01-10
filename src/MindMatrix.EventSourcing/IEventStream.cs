namespace MindMatrix.EventSourcing
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    public interface IEventStream
    {
        long Version { get; }

        Task<IAggregateStreamEvent<TEvent>> Append<TEvent>(TEvent data);
        IAsyncEnumerable<IAggregateStreamEvent> Read(int start = 0);
    }


    public class MemoryEventStream<TAggregate> : IEventStream
    {
        private class EventData
        {
            public string Id { get; }
            public long Version { get; }
            public string EventType { get; }
            public byte[] Data { get; }
            public byte[] Metadata { get; }
            public EventData(string id, long version, string type, byte[] data, byte[] metadata)
            {
                Id = id;
                Version = version;
                EventType = type;
                Data = data;
                Metadata = metadata;
            }
        }

        private readonly string _aggregateId;

        private long _version = -1;

        public long Version => _version;

        private List<EventData> _events = new List<EventData>();
        private readonly Assembly _assembly = typeof(TAggregate).Assembly;

        public MemoryEventStream(string aggregateId)
        {
            _aggregateId = aggregateId;
        }

        public Task<IAggregateStreamEvent<TEvent>> Append<TEvent>(TEvent data)
        {
            var type = data.GetType();
            var eid = Guid.NewGuid();
            var version = _version;
            var bytes = Json.ToJsonBytes(data);

            if (_events.Count - 1 != _version)
                throw new InvalidOperationException($"Optimistic Concurrency Check, expected: {_version}");

            _version++;
            _events.Add(new EventData(eid.ToString(), _version, type.FullName, bytes, null));
            return Task.FromResult<IAggregateStreamEvent<TEvent>>(new AggregateStreamEvent<TEvent>(type, eid.ToString(), version, bytes, data));
        }

        public async IAsyncEnumerable<IAggregateStreamEvent> Read(int start = 0)
        {
            await Task.CompletedTask;

            for (var i = start; i < _events.Count; i++)
            {
                var type = _assembly.GetType(_events[i].EventType);
                yield return new AggregateStreamEvent(type, _events[i].Id, _events[i].Version, _events[i].Data, _events[i].Metadata);
            }
        }
    }
}