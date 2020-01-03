namespace AggTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AggCommon;

    public class InMemoryAggregrateRepository<TEntity> : IAggregrateRepository<TEntity>
    //where TEntity : Aggregrate<TEntity>
    {

        //public string CollectionName { get; }

        private List<IAggregrate<TEntity>> _entities = new List<IAggregrate<TEntity>>();

        // public InMemoryAggregrateRepository()
        // {
        //     CollectionName = typeof(TEntity).Name;
        // }

        public Task<IAggregrate<TEntity>> AddAsync(TEntity entity)
        {
            var aggregate = new Aggregrate<TEntity>(Guid.NewGuid().ToString(), entity);
            _entities.Add(aggregate);
            return Task.FromResult<IAggregrate<TEntity>>(aggregate);
        }

        public Task DeleteAsync(IAggregrate<TEntity> entity)
        {
            var index = _entities.FindIndex(it => it.AggregrateId == entity.AggregrateId);
            if (index == -1)
                throw new AggregrateNotFound(entity.AggregrateId);

            _entities.RemoveAt(index);
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<IAggregrate<TEntity>> Find(Expression<Func<IAggregrate<TEntity>, bool>> filter)
        {
            var entities = _entities.AsQueryable().Where(filter);
            foreach (var entity in entities)
                yield return entity;

            await Task.CompletedTask;
        }

        public Task ReplaceAsync(IAggregrate<TEntity> entity)
        {
            var index = _entities.FindIndex(it => it.AggregrateId == entity.AggregrateId);
            if (index == -1)
                throw new AggregrateNotFound(entity.AggregrateId);

            _entities[index] = entity;
            return Task.CompletedTask;
        }
    }

}