using System;
using System.Threading.Tasks;
using AggCommon;

namespace AggTest
{
    public class InMemoryAggregateRepository<T> : IAggregateRepository<T>
        where T : new()
    {
        private IAggregateStream<T> _stream;

        public InMemoryAggregateRepository(IAggregateStream<T> stream)
        {
            _stream = stream;
        }

        public async Task<IAggregate<T>> GetById(string aggregateId)
        {
            var agg = new Aggregrate<T>(aggregateId, new T(), _stream);
            await foreach (var it in _stream.Read(aggregateId))
                it.Apply(agg.Root);
            return agg;
        }

        public Task<IAggregate<T>> Create()
        {
            return Task.FromResult<IAggregate<T>>(new Aggregrate<T>(Guid.NewGuid().ToString(), new T(), _stream));
        }
    }
}