namespace AggTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AggCommon;

    public class InMemoryAggregrateRepository<TEntity> : IAggregrateRepository<TEntity>
         where TEntity : IAggregrate
    {
        public string CollectionName { get; }

        private List<TEntity> _entities = new List<TEntity>();

        public InMemoryAggregrateRepository()
        {
            CollectionName = typeof(TEntity).Name;
        }

        public Task AddAsync(TEntity entity)
        {
            _entities.Add(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            var index = _entities.FindIndex(it => it.AggregrateId == entity.AggregrateId);
            if (index == -1)
                throw new AggregrateNotFound(entity.AggregrateId);

            _entities.RemoveAt(index);
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter)
        {
            var entities = _entities.AsQueryable().Where(filter);
            foreach (var entity in entities)
                yield return entity;

            await Task.CompletedTask;
        }

        public Task ReplaceAsync(TEntity entity)
        {
            var index = _entities.FindIndex(it => it.AggregrateId == entity.AggregrateId);
            if (index == -1)
                throw new AggregrateNotFound(entity.AggregrateId);

            _entities[index] = entity;
            return Task.CompletedTask;
        }
    }

}