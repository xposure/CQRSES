namespace MindMatrix.EventSourcing
{

    public interface IAggregateEvent<TAggregate> : IEvent<TAggregate>
    {
        string Id { get; }
        long Version { get; }

    }

    public interface IAggregateEvent<TAggregate, TData> : IAggregateEvent<TAggregate>
        where TData : IEvent<TAggregate>
    {
        TData Data { get; }
    }

    public class AggregateEvent<TAggregate> : IAggregateEvent<TAggregate, IEvent<TAggregate>>
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

    public class AggregateEvent<TAggregate, TEvent> : IAggregateEvent<TAggregate, TEvent>
        where TEvent : IEvent<TAggregate>
    {
        public string Id { get; }
        public long Version { get; }
        public TEvent Data { get; }

        public AggregateEvent(string id, long version, TEvent data)
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
}