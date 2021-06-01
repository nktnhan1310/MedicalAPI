﻿using AutoMapper;
using Medical.Entities;
using Medical.Entities.DomainEntity;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Interface
{
    public interface IDomainService<E, T> where E : MedicalAppDomain where T : BaseSearch
    {
        bool Save(E item);
        bool Save(IList<E> items);
        bool Delete(int id);
        IList<E> GetAll();
        E GetById(int id);
        Task<E> GetByIdAsync(int id);
        E GetById(int id, Expression<Func<E, E>> select);
        E GetById(int id, IConfigurationProvider mapperConfiguration);
        Task<E> GetByIdAsync(int id, IConfigurationProvider mapperConfiguration);

        IList<E> GetAll(Expression<Func<E, E>> select);
        IList<E> Get(Expression<Func<E, bool>> expression, Expression<Func<E, E>> select);
        IList<E> Get(Expression<Func<E, bool>>[] expression, Expression<Func<E, E>> select);
        IList<E> Get(Expression<Func<E, bool>> expression);
        IList<E> Get(Expression<Func<E, bool>>[] expression);
        IList<E> Get(Expression<Func<E, bool>> expression, IConfigurationProvider mapperConfiguration);
        IList<E> Get(Expression<Func<E, bool>>[] expression, IConfigurationProvider mapperConfiguration);

        Task<PagedList<E>> GetPagedListData(T baseSearch);

        Task<bool> SaveAsync(E item);
        Task<bool> SaveAsync(IList<E> items);
        Task<bool> DeleteAsync(int id);
        Task<IList<E>> GetAllAsync();
        Task<E> GetByIdAsync(int id, Expression<Func<E, E>> select);
        Task<IList<E>> GetAllAsync(Expression<Func<E, E>> select);
        Task<IList<E>> GetAsync(Expression<Func<E, bool>> expression, Expression<Func<E, E>> select);
        Task<IList<E>> GetAsync(Expression<Func<E, bool>>[] expression, Expression<Func<E, E>> select);
        Task<IList<E>> GetAsync(Expression<Func<E, bool>> expression);
        Task<IList<E>> GetAsync(Expression<Func<E, bool>>[] expression);
        Task<IList<E>> GetAsync(Expression<Func<E, bool>> expression, bool useProjectTo);
        Task<IList<E>> GetAsync(Expression<Func<E, bool>>[] expression, bool useProjectTo);

        Task<bool> UpdateAsync(E item);
        Task<bool> UpdateAsync(IList<E> items);
        Task<bool> CreateAsync(E item);
        Task<bool> CreateAsync(IList<E> items);

        Task<bool> UpdateFieldAsync(IList<E> items, params Expression<Func<E, object>>[] includeProperties);
        Task<bool> UpdateFieldAsync(E item, params Expression<Func<E, object>>[] includeProperties);

        /// <summary>
        /// Check item có tồn tại không
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<string> GetExistItemMessage(E item);
       
    }
}
