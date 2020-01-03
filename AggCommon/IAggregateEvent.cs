namespace AggCommon
{
    using System;

    public interface IEventState<TAggregate>
    {
        void Apply(TAggregate aggregreate);

    }

    public interface IAggregrateEvent<TAggregate>
    {
        string Id { get; }
        long Index { get; }

        void Apply(TAggregate aggregreate);
    }

    public interface IAggregrateEvent<TAggregate, TData> : IAggregrateEvent<TAggregate>
        where TData : IEventState<TAggregate>
    {
        TData Data { get; }
    }
}