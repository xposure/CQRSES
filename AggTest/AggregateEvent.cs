namespace AggTest
{
    using System;
    using AggCommon;

    // public abstract class AggregrateEvent<TEntity> : IAggregrateEvent<TEntity>
    // //where TEntity : Aggregrate<TEntity>
    // {
    //     public string Id { get; }
    //     public long Index { get; }

    //     protected AggregrateEvent(string id, long index)
    //     {
    //         Id = id;
    //         Index = index;
    //     }

    //     protected abstract void OnApply(TEntity aggregreate);

    //     public void Apply(TEntity aggregreate)
    //     {
    //         if (Index == -1)
    //             throw new InvalidOperationException("Event was not properly initialized.\nIndex can not be -1.");

    //         if (string.IsNullOrEmpty(Id))
    //             throw new InvalidOperationException("Event was not properly initialized.\nId can not be null.");

    //         ev.Apply(aggregrate.Root);
    //         aggregrate.EventIndex = Index;
    //     }
    // }

    public class AggregrateEvent<TEntity, TData> : IAggregrateEvent<TEntity, TData>
        //where TEntity : IAggregrate<TEntity>
        where TData : IEventState<TEntity>
    {
        public string Id { get; }
        public long Index { get; }
        public TData Data { get; }

        public IAggregrate<TEntity> Entity { get; }

        public AggregrateEvent(IAggregrate<TEntity> entity, string id, long index, TData data)
        {
            Entity = entity;
            Id = id;
            Index = index;
            Data = data;
        }

        public void Apply()
        {
            Data.Apply(Entity.Root);
            //Entity.Apply(this);
        }

        //public void Apply(TEntity aggregrate)
        // {
        //     Data.Apply(aggregrate);
        // }
    }
}