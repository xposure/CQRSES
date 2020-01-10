using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindMatrix.EventSourcing
{
    public class GrpcEventStream<T> : IEventStream
    {
        public long Version => throw new System.NotImplementedException();

        public Task<IAggregateStreamEvent<TEvent>> Append<TEvent>(TEvent data)
        {
            throw new System.NotImplementedException();
        }

        public IAsyncEnumerable<IAggregateStreamEvent> Read(int start = 0)
        {
            throw new System.NotImplementedException();
        }
    }
}