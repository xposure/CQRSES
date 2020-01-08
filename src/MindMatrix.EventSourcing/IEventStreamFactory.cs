namespace MindMatrix.EventSourcing
{
    using EventStore.ClientAPI;

    public interface IEventStreamFactory<TAggregate>
    {
        IEventStream<TAggregate> Create(string aggregateId);
    }

    public class EventStreamFactory<TAggregate> : IEventStreamFactory<TAggregate>
    {
        private readonly IEventStoreConnection _eventStore;
        public EventStreamFactory(IEventStoreConnection eventStore)
        {
            _eventStore = eventStore;
        }

        public IEventStream<TAggregate> Create(string aggregateId)
        {
            return new EventStream<TAggregate>(_eventStore, aggregateId);
        }
    }

    public class MemoryEventStreamFactory<TAggregate> : IEventStreamFactory<TAggregate>
    {
        private readonly IEventStoreConnection _eventStore;
        public MemoryEventStreamFactory(IEventStoreConnection eventStore)
        {
            _eventStore = eventStore;
        }

        public IEventStream<TAggregate> Create(string aggregateId)
        {
            return new MemoryEventStream<TAggregate>(aggregateId);
        }
    }
}
