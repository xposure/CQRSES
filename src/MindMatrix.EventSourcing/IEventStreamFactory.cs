namespace MindMatrix.EventSourcing
{
    using EventStore.ClientAPI;
    using MediatR;

    public interface IEventStreamFactory<TAggregate>
    {
        IEventStream Create(string aggregateId);
    }

    public class EventStreamFactory<TAggregate> : IEventStreamFactory<TAggregate>
    {
        private readonly IEventStoreConnection _eventStore;

        public EventStreamFactory(IEventStoreConnection eventStore)
        {
            _eventStore = eventStore;
        }

        public IEventStream Create(string aggregateId)
        {
            return new EventStream<TAggregate>(_eventStore, aggregateId);
        }


    }

    public class MemoryEventStreamFactory<TAggregate> : IEventStreamFactory<TAggregate>
    {
        public MemoryEventStreamFactory()
        {
        }

        public IEventStream Create(string aggregateId)
        {
            return new MemoryEventStream<TAggregate>(aggregateId);
        }
    }
}
