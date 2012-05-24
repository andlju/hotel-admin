using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web;
using Petite;

namespace HotelAdmin.DataAccess
{
    /// <summary>
    /// This will act as an IObjectSetProvider and IObjectContext using a per WCF-operation ObjectContext that is
    /// automatically disposed of when completed.
    /// </summary>
    public class AspNetObjectContextAdapter : IDbSetProvider, IObjectContext
    {
        private readonly IDbContextFactory _contextFactory;

        /// <summary>
        /// Create a new instance of the adapter
        /// </summary>
        /// <param name="contextFactory">Factory to use to construct the DbContext</param>
        public AspNetObjectContextAdapter(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        protected virtual string KeyName { get { return _contextFactory.ContextTypeName; } }

        public IDbSet<TEntity> CreateDbSet<TEntity>() where TEntity : class
        {
            var context = GetCurrentContext();
            return context.Set<TEntity>();
        }

        public int SaveChanges()
        {
            var context = GetCurrentContext();

            return context.SaveChanges();
        }

        public void Detach(object obj)
        {
            /*var context = GetCurrentContext();
            context.(obj);*/
        }

        protected DbContext GetCurrentContext()
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
                throw new InvalidOperationException("The AspNetObjectContextAdapter can only be used in an HttpContext.");

            var dbContext = httpContext.Items["AspNetObjectContextAdapter_DbContext_" + KeyName] as DbContext;

            if (dbContext == null)
            {
                // No DbContext has been created for this operation yet. Create a new one...
                dbContext = _contextFactory.Create();

                // ... store it ...
                httpContext.Items["AspNetObjectContextAdapter_DbContext_" + KeyName] = dbContext;
            }

            return dbContext;
        }
    }
}