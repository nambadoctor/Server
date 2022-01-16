﻿using MongoDB.Driver;
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
            Context.AddCommand(async () => await DbSet.InsertOneAsync(obj));
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
            foreach (var item in list)
            {
                result.AddRange(item);
            }
            return result;
        }

        public async Task<T> GetSingleByFilterAndProject<T>(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity, IEnumerable<T>> project)
        {
            var result = await DbSet.Aggregate().Match(filter).Project(project).Limit(1).SingleOrDefaultAsync();
            var singleObject = result;
            if (singleObject != null)
            {
                return singleObject.FirstOrDefault();
            }
            else
            {
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

        public virtual void Remove(string id)
        {
            Context.AddCommand(() => DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id)));
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
    }
}