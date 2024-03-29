﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task Add(TEntity obj);
        Task AddMany(List<TEntity> objs);
        Task<TEntity> GetById(string id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> GetListByFilter(FilterDefinition<TEntity> filter);
        Task<TEntity> GetSingleByFilter(FilterDefinition<TEntity> filter);
        Task<IEnumerable<T>> GetListByFilterAndProject<T>(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity, IEnumerable<T>> project);
        Task<List<TEntity>> GetProjectedListByFilterAndProject(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity> project);
        Task<T> GetSingleByFilterAndProject<T>(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity, IEnumerable<T>> project);
        Task<T> GetSingleNestedByFilterAndProject<T>(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity, T> project);
        Task AddToSet(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> updateDefinition);
        Task RemoveFromSet(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> updateDefinition);
        Task Upsert(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> updateDefinition);
        void Update(TEntity obj);
        Task Remove(string id);
        Task RemoveWithFilter(FilterDefinition<TEntity> filter);
    }
}