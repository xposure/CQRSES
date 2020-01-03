namespace AggTest
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggCommon;

    public class InMemoryAggregrateEvents<T> : IAggregrateEvents<T>
    {
        public readonly Dictionary<string, List<IAggregrateEvent<T>>> Events = new Dictionary<string, List<IAggregrateEvent<T>>>();

        public Task Append<TEvent>(string aggregrateId, TEvent eventData) where TEvent : IAggregrateEvent<T>
        {
            if (!Events.TryGetValue(aggregrateId, out var eventList))
            {
                eventList = new List<IAggregrateEvent<T>>();
                Events.Add(aggregrateId, eventList);
            }

            eventList.Add(eventData);
            return Task.CompletedTask;
        }
    }
}