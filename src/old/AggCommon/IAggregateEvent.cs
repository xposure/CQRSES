namespace AggCommon
{
    using System;

    public interface IEventState
    {
        void Apply();
    }

    public interface IEventState<TAggregate>
    {
        void Apply(TAggregate aggregreate);
    }


    public interface IAggregateEvent
    {
        string Id { get; }
        long Index { get; }

        //void Apply();
    }

    public interface IAggregateEvent<TAggregate> : IAggregateEvent
    {
        void Apply(TAggregate aggregreate);
    }

    public interface IAggregateEvent<TAggregate, TData> : IAggregateEvent<TAggregate>
        //where TAggregate : IAggregrate
        where TData : IEventState<TAggregate>
    {
        TData Data { get; }
    }
}