namespace AggCommon
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IAggregrateRepository<TEntity>
       where TEntity : IAggregrate
    {
        //string CollectionName { get; }

        Task AddAsync(TEntity entity);
        Task ReplaceAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        IAsyncEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter);
    }
}