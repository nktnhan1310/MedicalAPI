using Medical.Service.DbExtensions;
using Medical.Service.Factory;
using Medical.Entities.DomainEntity;
using Medical.Interface.DbContext;
using Medical.Interface.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.Data.SqlClient;

//using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medical.Utilities;
using System.Linq.Expressions;

namespace Medical.Service
{
    public class DomainRepository<T> : IDomainRepository<T> where T : MedicalAppDomain
    {
        protected readonly IMedicalDbContext Context;

        public DomainRepository(IMedicalDbContext context)
        {
            Context = context;
        }
        public DomainRepository(IDbContextFactory dbContextFactory)
        {
            Context = dbContextFactory.Create();
        }


        public virtual void SetEntityState(T item, EntityState entityState)
        {
            Context.Entry(item).State = entityState;
        }

        public virtual void Attach(T item)
        {
            Context.Set<T>().Attach(item);
            Context.Entry<T>(item).State = EntityState.Unchanged;
        }

        public virtual void Create(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public virtual async Task CreateAsync(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
        }

        public virtual void Create(IList<T> entities)
        {
            Context.Set<T>().AddRange(entities);
        }
        public virtual async Task CreateAsync(IList<T> entities)
        {
            await Context.Set<T>().AddRangeAsync(entities);
        }

        public virtual void AddOrUpdate(T entity)
        {
            Context.Set<T>().AddOrUpdate<T>(entity);
        }

        public virtual void Update(T entity)
        {
            //Context.Entry(entity).State = EntityState.Modified;
            Context.Set<T>().Update(entity);
        }

        /// <summary>
        /// Update Field entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual bool UpdateFieldsSave(T entity, params Expression<Func<T, object>>[] includeProperties)
        {
            var dbEntry = Context.Entry(entity);

            foreach (var includeProperty in includeProperties)
            {
                dbEntry.Property(includeProperty).IsModified = true;
            }
            Context.SaveChanges();
            return true;
        }

        public virtual void Detach(T entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        public virtual void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public virtual void Delete(IList<T> entities)
        {
            Context.Set<T>().RemoveRange(entities);
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return Context.Set<T>().Where(e => !e.Deleted);
        }

        public virtual void LoadReference(T item, params string[] property)
        {
            foreach (var prop in property)
            {
                Context.Entry(item).Reference(prop).Load();
            }
        }

        
        public virtual void LoadCollection(T item, params string[] property)
        {
            foreach (var prop in property)
            {
                Context.Entry(item).Collection(prop).Load();
            }
        }

        /// <summary>
        /// Lấy danh sách phân trang
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public Task<PagedList<T>> ExcuteQueryPagingAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return Task.Run(() =>
            {
                PagedList<T> pagedList = new PagedList<T>();
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(sqlParameters);
                    command.Parameters["@TotalPage"].Direction = ParameterDirection.Output;
                    command.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    pagedList.TotalItem = int.Parse(command.Parameters["@TotalPage"].Value.ToString());
                    pagedList.Items = MappingDataTable.ConvertToList<T>(dataTable);
                    return pagedList;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }

        public Task<DataTable> ExcuteQueryAsync(string commandText, SqlParameter[] sqlParameters)
        {
            return Task.Run(() =>
            {
                DataTable dataTable = new DataTable();
                SqlConnection connection = null;
                SqlCommand command = null;
                try
                {
                    connection = (SqlConnection)Context.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(sqlParameters);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    return dataTable;
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
            });
        }

        public DataTable ExcuteQuery(string commandText, SqlParameter[] sqlParameters)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                connection = (SqlConnection)Context.Database.GetDbConnection();
                command = connection.CreateCommand();
                connection.Open();
                command.CommandText = commandText;
                command.Parameters.AddRange(sqlParameters);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
                sqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
            finally
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                if (command != null)
                    command.Dispose();
            }
        }

        public int ExecuteNonQuery(string commandText, SqlParameter[] sqlParameters)
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                connection = (SqlConnection)Context.Database.GetDbConnection();
                command = connection.CreateCommand();
                connection.Open();
                command.CommandText = commandText;
                command.Parameters.AddRange(sqlParameters);
                return command.ExecuteNonQuery();
            }
            finally
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                if (command != null)
                    command.Dispose();
            }
        }

        public int ExecuteNonQuery(string commandText)
        {
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                connection = (SqlConnection)Context.Database.GetDbConnection();
                command = connection.CreateCommand();
                connection.Open();
                command.CommandText = commandText;
                return command.ExecuteNonQuery();
            }
            finally
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                    connection.Close();

                if (command != null)
                    command.Dispose();
            }
        }

    }
}
