// namespace AggService
// {
//     using System;
//     using System.Collections.Generic;
//     using System.Linq.Expressions;
//     using System.Threading.Tasks;
//     using AggCommon;
//     using MongoDB.Driver;

//     public class MongoAggregrateRepository<TEntity> : IAggregrateRepository<TEntity>
//         where TEntity : IAggregrate
//     {
//         private readonly IMongoCollection<TEntity> _collection;

//         public MongoAggregrateRepository(IMongoCollection<TEntity> collection)
//         {
//             if (null == collection)
//                 throw new ArgumentNullException("collection");
//             _collection = collection;

//             this.CollectionName = collection.CollectionNamespace.CollectionName;
//         }

//         public string CollectionName { get; private set; }

//         public Task AddAsync(TEntity entity)
//         {
//             return _collection.InsertOneAsync(entity);
//         }

//         public Task DeleteAsync(TEntity entity)
//         {
//             return _collection.DeleteOneAsync(it => it.AggregrateId == entity.AggregrateId);
//         }

//         public Task ReplaceAsync(TEntity entity)
//         {
//             return _collection.FindOneAndReplaceAsync(it => it.AggregrateId == entity.AggregrateId, entity);
//         }

//         public async IAsyncEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> filter)
//         {
//             var cursor = await _collection.FindAsync(filter);
//             while (await cursor.MoveNextAsync())
//             {
//                 foreach (var entity in cursor.Current)
//                 {
//                     yield return entity;
//                 }
//             }
//         }
//     }
// }