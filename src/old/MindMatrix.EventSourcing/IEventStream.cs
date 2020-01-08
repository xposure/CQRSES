namespace MindMatrix.EventSourcing
{
    using System.Collections.Generic;

    public interface IEventStream<T>
    {
        IAsyncEnumerable<T> GetByAggregate(string aggregateId);

    }
}