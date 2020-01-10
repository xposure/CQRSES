namespace MindMatrix.EventSourcing
{
    using MediatR;

    public interface IEventStreamFactory<TAggregate>
    {
        IEventStream Create(string aggregateId);
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
