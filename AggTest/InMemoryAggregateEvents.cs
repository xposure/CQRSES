namespace AggTest
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggCommon;

    public class InMemoryAggregrateEvents<T> : IAggregrateEvents<T>
    //where T : IAggregrate<T>
    {
        public readonly Dictionary<string, List<IAggregrateEvent<T>>> Events = new Dictionary<string, List<IAggregrateEvent<T>>>();

        public Task<IAggregrateEvent<T>> Append<TEvent>(IAggregrate<T> aggregrate, TEvent eventData)
            where TEvent : IEventState<T>
        {
            if (!Events.TryGetValue(aggregrate.AggregrateId, out var eventList))
            {
                eventList = new List<IAggregrateEvent<T>>();
                Events.Add(aggregrate.AggregrateId, eventList);
            }

            var ev = new AggregrateEvent<T, TEvent>(Guid.NewGuid().ToString(), eventList.Count, eventData);
            eventList.Add(ev);
            return Task.FromResult<IAggregrateEvent<T>>(ev);
        }
    }
}