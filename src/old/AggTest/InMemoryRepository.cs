namespace AggTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AggCommon;
    using AggRepo;

    public class InMemoryRepository<TEntity> : IRepository<TEntity>
        where TEntity : new()
        //where TEntity : Aggregrate<TEntity>
    {

        //public string CollectionName { get; }

        private List<IAggregate<TEntity>> _entities = new List<IAggregate<TEntity>>();

        // public InMemoryAggregrateRepository()
        // {
        //     CollectionName = typeof(TEntity).Name;
        // }
        private IAggregateStream<TEntity> _stream;
        public InMemoryRepository(IAggregateStream<TEntity> stream)
        {
            _stream = stream;
        }

        public Task<IAggregate<TEntity>> AddAsync(TEntity entity)
        {
            var aggregate = new Aggregrate<TEntity>(Guid.NewGuid().ToString(), entity, _stream);
            _entities.Add(aggregate);
            return Task.FromResult<IAggregate<TEntity>>(aggregate);
        }

        public Task DeleteAsync(IAggregate<TEntity> entity)
        {
            var index = _entities.FindIndex(it => it.AggregateId == entity.AggregateId);
            if (index == -1)
                throw new AggregateNotFound(entity.AggregateId);

            _entities.RemoveAt(index);
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<IAggregate<TEntity>> Find(Expression<Func<IAggregate<TEntity>, bool>> filter)
        {
            var entities = _entities.AsQueryable().Where(filter);
            foreach (var entity in entities)
                yield return entity;

            await Task.CompletedTask;
        }

        public Task ReplaceAsync(IAggregate<TEntity> entity)
        {
            var index = _entities.FindIndex(it => it.AggregateId == entity.AggregateId);
            if (index == -1)
                throw new AggregateNotFound(entity.AggregateId);

            _entities[index] = entity;
            return Task.CompletedTask;
        }
    }

}