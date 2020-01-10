using System;

namespace MindMatrix.EventSourcing
{
    public interface IAggregateStreamEvent : IAggregateEvent
    {
        Type Type { get; }
        byte[] Bytes { get; }
    }

    public interface IAggregateStreamEvent<TData> : IAggregateStreamEvent, IAggregateEvent<TData>
    {
    }

    public class AggregateStreamEvent : IAggregateStreamEvent
    {
        public string Id { get; }
        public long Version { get; }

        public byte[] Bytes { get; }

        public byte[] Metadata { get; }

        public Type Type { get; }

        public AggregateStreamEvent(Type type, string id, long version, byte[] data, byte[] metadata)
        {
            Type = type;
            Id = id;
            Version = version;
            Bytes = data;
            Metadata = metadata;
        }
    }

    public class AggregateStreamEvent<TData> : IAggregateStreamEvent<TData>
    {
        public string Id { get; }
        public long Version { get; }

        public byte[] Bytes { get; }
        public TData Data { get; }

        public Type Type { get; }

        public AggregateStreamEvent(Type type, string id, long version, byte[] bytes, TData ev)
        {
            Type = type;
            Id = id;
            Version = version;
            Bytes = bytes;
            Data = ev;
        }
    }

}