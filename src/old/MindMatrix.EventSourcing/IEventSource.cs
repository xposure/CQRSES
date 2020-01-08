namespace MindMatrix.EventSourcing
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EventStore.ClientAPI;
    public interface IAggregateEvent
    {

    }

    public class AggregateEvent<TAggregate>
    {

    }

    public interface IAggregateEventFactory<TAggregate>
    {
        AggregateEvent<TAggregate> Create(string type);
    }




    public class EventSource<TAggregate>
    {
        private readonly IEventStoreConnection _eventStore;
        private readonly IAggregateEventFactory<TAggregate> _eventFactory;
        public EventSource(IEventStoreConnection eventStore, IEventFactory<TAggregate> eventFactory)
        {
            _eventStore = eventStore;
        }

        public async IAsyncEnumerable<AggregateEvent<TAggregate>> ReadEvents(string aggregateId)
        {
            await foreach (var resolvedEvent in _eventStore.ReadEventsAsync(aggregateId))
            {
                var aggEvent = _eventFactory.Create(resolvedEvent.Event.EventType);
                yield return aggEvent;
            }
        }
    }



    public interface IEventSource2
    {
        string Id { get; }
        long Version { get; }


        void Snapshot();
    }

    public interface IEventSource2<TAggregate> : IEventSource2
    {
        TAggregate Root { get; }

        //IReadOnlyList<IEvent> Events { get; }
        Task Append(params IEvent<TAggregate>[] data);
    }
}