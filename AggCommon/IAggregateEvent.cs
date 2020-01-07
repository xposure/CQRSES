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


    public interface IAggregrateEvent
    {
        string Id { get; }
        long Index { get; }

        void Apply();
    }

    // public interface IAggregrateEvent<TData> : IAggregrateEvent
    // {
    //     TData Data { get; }
    //     //void Apply(TAggregate aggregreate);
    // }

    public interface IAggregrateEvent<TAggregate, TData> : IAggregrateEvent
        //where TAggregate : IAggregrate
        where TData : IEventState<TAggregate>
    {
        TData Data { get; }
    }
}