﻿using Medical.Entities.DomainEntity;
using Medical.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
//using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Medical.Interface.Repository
{
    public interface IDomainRepository<T> where T : MedicalAppDomain
    {
        IQueryable<T> GetQueryable();
        void Create(T entity);
        Task CreateAsync(T entity);
        void Create(IList<T> entities);
        Task CreateAsync(IList<T> entities);
        void AddOrUpdate(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(IList<T> entities);
        void Attach(T item);
        void Detach(T entity);
        void SetEntityState(T item, EntityState entityState);
        void LoadReference(T item, params string[] property);
        void LoadCollection(T item, params string[] property);
        int ExecuteNonQuery(string commandText, SqlParameter[] sqlParameters);
        int ExecuteNonQuery(string commandText);

        /// <summary>  
        /// Adds the range.  
        /// </summary>  
        /// <param name="entities">The entities.</param>  
        void AddRange(IList<T> entities);

        /// <summary>  
        /// Removes the specified entity.  
        /// </summary>  
        /// <param name="entity">The entity.</param>  
        void RemoveRange(T entity);


        DataTable ExcuteQuery(string commandText, SqlParameter[] sqlParameters);

        Task<PagedList<T>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters);
        Task<DataTable> ExcuteQueryAsync(string commandText, SqlParameter[] sqlParameters);
        bool UpdateFieldsSave(T entity, params Expression<Func<T, object>>[] includeProperties);
        Task<IList<T>> ExcuteStoreAsync(string commandText, SqlParameter[] sqlParameters);

        Task<object> ExcuteStoreGetValue(string commandText, SqlParameter[] sqlParameters, string outputName);
        Task<bool> UpdateFieldsSaveAsync(T entity, params Expression<Func<T, object>>[] includeProperties);
    }
}
