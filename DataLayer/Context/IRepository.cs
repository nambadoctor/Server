using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.GenericRepository.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        void Add(TEntity obj);
        Task<TEntity> GetById(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> GetListByFilter(FilterDefinition<TEntity> filter);
        Task<TEntity> GetSingleByFilter(FilterDefinition<TEntity> filter);
        Task<IEnumerable<T>> GetListByFilterAndProject<T>(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity, IEnumerable<T>> project);
        Task<TEntity> GetSingleByFilterAndProject(FilterDefinition<TEntity> filter, ProjectionDefinition<TEntity> project);
        Task AddToSet(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> updateDefinition);
        Task Upsert(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> updateDefinition);
        void Update(TEntity obj);
        void Remove(Guid id);
    }
}