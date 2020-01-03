namespace AggTest
{
    using System;
    using AggCommon;

    public class Aggregrate<T> : IAggregrate<T>
    //where T : IAggregrateEvent<T>
    {
        public string AggregrateId { get; internal set; }
        public long EventIndex { get; internal set; }

        public T Root { get; }

        public Aggregrate(string aggregrateId, T root)
        {
            AggregrateId = aggregrateId;
            Root = root;
        }

        public void Apply(IAggregrateEvent<T> ev)
        {
            if (ev.Index == -1)
                throw new InvalidOperationException("Event was not properly initialized.\nIndex can not be -1.");

            if (string.IsNullOrEmpty(ev.Id))
                throw new InvalidOperationException("Event was not properly initialized.\nId can not be null.");

            ev.Apply(Root);
            //Data.Apply(aggregrate.Root);
            EventIndex = ev.Index;
        }
    }
}