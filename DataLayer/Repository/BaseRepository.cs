using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Repository
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IMongoContext Context;
        protected IMongoCollection<TEntity> DbSet;

        protected BaseRepository(IMongoContext context)
        {
            Context = context;

            string collectionName = typeof(TEntity).Name + "s";
            DbSet = Context.GetCollection<TEntity>(collectionName);
        }

        public virtual async Task Add(TEntity obj)
        {
            await DbSet.InsertOneAsync(obj);
        }

        public virtual async Task AddMany(List<TEntity> objs)
        {
            await DbSet.InsertManyAsync(objs);
        }

        public virtual async Task<TEntity> GetById(string id)
        {
            var data = await DbSet.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));
            return data.SingleOrDefault();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
            return all.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetListByFilter(FilterDefinition<TEntity> filter)
        {
            var all = await DbSet.Find(filter).ToListAsync();
            return all;
        }

        public virtual async Task<TEntity> GetSingleByFilter(FilterDefinition<TEntity> filter)
        {
            var result = await DbSet.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetListByFilterAndProject<T>(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity, IEnumerable<T>> project)
        {
            var list = await DbSet.Aggregate().Match(filter).Project(project).ToListAsync();
            var result = new List<T>();

            if (list != null)
                foreach (var item in list)
                {
                    result.AddRange(item);
                }
            return result;
        }

        public async Task<T> GetSingleByFilterAndProject<T>(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity, IEnumerable<T>> project)
        {
            try
            {
                var result = await DbSet.Aggregate().Match(filter).Project(project).SingleOrDefaultAsync();

                var singleObject = result.FirstOrDefault();
                if (singleObject != null)
                {
                    return singleObject;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception e)
            {
                string str = e.Message;
                return default(T);
            }

        }

        public async Task AddToSet(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> updateDefinition)
        {
            await DbSet.UpdateOneAsync(filter, updateDefinition, new UpdateOptions { IsUpsert = true });
        }

        public async Task RemoveFromSet(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> updateDefinition)
        {
            await DbSet.UpdateOneAsync(filter, updateDefinition, new UpdateOptions { IsUpsert = true });
        }

        public async Task Upsert(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> updateDefinition)
        {
            await DbSet.UpdateOneAsync(filter, updateDefinition, new UpdateOptions { IsUpsert = true });
        }
        public virtual void Update(TEntity obj)
        {
            //Context.AddCommand(() => DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.GetId()), obj));
        }

        public async Task Remove(string id)
        {
            await DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public async Task<T> GetSingleNestedByFilterAndProject<T>(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity, T> project)
        {
            var result = await DbSet.Aggregate().Match(filter).Project(project).SingleOrDefaultAsync();
            return result;
        }

        public async Task<List<TEntity>> GetProjectedListByFilterAndProject(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity> project)
        {
            var docList = await DbSet.Find(filter).Project(project).ToListAsync();

            var entityList = new List<TEntity>();

            if (docList != null)
                foreach (var doc in docList)
                {
                    var entity = BsonSerializer.Deserialize<TEntity>(doc);
                    entityList.Add(entity);
                }
            return entityList;

        }

        public async Task RemoveWithFilter(FilterDefinition<TEntity> filter)
        {
            await DbSet.DeleteManyAsync(filter);
        }
    }
}