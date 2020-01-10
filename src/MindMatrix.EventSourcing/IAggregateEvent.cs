using System;
using MediatR;

namespace MindMatrix.EventSourcing
{

    public interface IAggregateEvent
    {
        string Id { get; }
        long Version { get; }
    }

    // public interface IAggregateEvent<TAggregate> : IAggregateEvent
    // {
    // }

    public interface IAggregateEvent<TData> : IAggregateEvent
    {
        TData Data { get; }
    }

    public interface IAggregateEvent<TAggregate, TData> : IAggregateEvent<TData>, INotification
    {
        IAggregate<TAggregate> Aggregate { get; }
    }



    public class AggregateEvent<TAggregate, TData> : IAggregateEvent<TAggregate, TData>
    {
        public string Id { get; }
        public long Version { get; }

        public IAggregate<TAggregate> Aggregate { get; }
        public TData Data { get; }

        public AggregateEvent(IAggregate<TAggregate> aggregate, string id, long version, TData data)
        {
            Aggregate = aggregate;
            Id = id;
            Version = version;
            Data = data;
        }
    }

}