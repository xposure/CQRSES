namespace AggRepo
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AggCommon;

    public interface IAggregrateRepository<TEntity>
    //where TEntity : IAggregrate
    {
        //string CollectionName { get; }

        Task<IAggregrate<TEntity>> AddAsync(TEntity entity);

        Task ReplaceAsync(IAggregrate<TEntity> entity);
        Task DeleteAsync(IAggregrate<TEntity> entity);
        IAsyncEnumerable<IAggregrate<TEntity>> Find(Expression<Func<IAggregrate<TEntity>, bool>> filter);
    }
}