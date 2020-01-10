namespace MindMatrix.EventSourcing
{
    using EventStore.ClientAPI;

    public class EventStoreEventStreamFactory<TAggregate> : IEventStreamFactory<TAggregate>
    {
        private readonly IEventStoreConnection _eventStore;

        public EventStoreEventStreamFactory(IEventStoreConnection eventStore)
        {
            _eventStore = eventStore;
        }

        public IEventStream Create(string aggregateId)
        {
            return new EventStoreEventStream<TAggregate>(_eventStore, aggregateId);
        }
    }
}
