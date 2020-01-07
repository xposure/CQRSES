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

        Task<IAggregate<TEntity>> AddAsync(TEntity entity);

        Task ReplaceAsync(IAggregate<TEntity> entity);
        Task DeleteAsync(IAggregate<TEntity> entity);
        IAsyncEnumerable<IAggregate<TEntity>> Find(Expression<Func<IAggregate<TEntity>, bool>> filter);
    }
}