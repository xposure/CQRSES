namespace AggCommon
{
    using System;

    public interface IAggregrateEvent<T>
    {
        string Id { get; }
        void Apply(T aggregreate);
    }



    public abstract class AggregrateEvent<T> : IAggregrateEvent<T>
        where T : Aggregrate
    {
        public string Id { get; internal set; }
        public long Index { get; internal set; }

        protected AggregrateEvent()
        {

        }

        public AggregrateEvent(string id, long index)
        {
            Id = id;
            Index = index;
        }

        public void Apply(T aggregrate)
        {
            if (Index == -1)
                throw new InvalidOperationException("Event was not properly initialized.\nIndex can not be -1.");

            if (string.IsNullOrEmpty(Id))
                throw new InvalidOperationException("Event was not properly initialized.\nId can not be null.");

            OnApply(aggregrate);
            aggregrate.EventIndex = Index;
        }

        protected abstract void OnApply(T aggregrate);
    }
}