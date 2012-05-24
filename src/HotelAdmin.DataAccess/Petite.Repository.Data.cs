using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;

namespace Petite
{
    /// <summary>
    /// Interface implemented by the Context class to provide an Object Set
    /// </summary>
    public interface IDbSetProvider
    {
        /// <summary>
        /// Create a Db Set for a specific entity
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to provide a set for</typeparam>
        /// <returns>An object set for <typeparamref name="TEntity"/> objects</returns>
        IDbSet<TEntity> CreateDbSet<TEntity>() where TEntity : class;
    }

    /// <summary>
    /// Interface for creating an object context
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// Create an Entity Framework object context
        /// </summary>
        /// <returns>A new DbContext</returns>
        DbContext Create();

        /// <summary>
        /// Identifies this Db Context
        /// </summary>
        string ContextTypeName { get; }
    }

    /// <summary>
    /// Factory for creating an Entity Framework ObjectContext
    /// </summary>
    /// <typeparam name="TContext">Type of ObjectContext to create</typeparam>
    public class DbContextFactory<TContext> : IDbContextFactory
        where TContext : DbContext, new()
    {
        public DbContext Create()
        {
            return new TContext();
        }

        public string ContextTypeName
        {
            get { return typeof(TContext).Name; }
        }
    }

    /// <summary>
    /// This will act as an IObjectSetProvider and IObjectContext using a per WCF-operation ObjectContext that is
    /// automatically disposed of when completed.
    /// </summary>
    public class WcfObjectContextAdapter : IDbSetProvider, IObjectContext
    {
        private readonly IDbContextFactory _contextFactory;

        /// <summary>
        /// Create a new instance of the adapter
        /// </summary>
        /// <param name="contextFactory">Factory to use to construct the DbContext</param>
        public WcfObjectContextAdapter(IDbContextFactory contextFactory)
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
            var storageExtension = OperationContextStorageExtension.Current;

            var dbContext = storageExtension.Get<DbContext>(KeyName);

            if (dbContext == null)
            {
                // No DbContext has been created for this operation yet. Create a new one...
                dbContext = _contextFactory.Create();
                // ... store it and make sure it will be Disposed when the operation is completed.
                storageExtension.Store(dbContext, KeyName, () => dbContext.Dispose());
            }

            return dbContext;
        }
    }

    /// <summary>
    /// A simple implementation of IObjectContext and IObjectSetProvider that creates a single object context
    /// and uses it
    /// </summary>
    public class SingleUsageObjectContextAdapter : IDbSetProvider, IObjectContext, IDisposable
    {
        private readonly IDbContextFactory _contextFactory;
        protected DbContext DbContext { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextFactory"></param>
        public SingleUsageObjectContextAdapter(IDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            DbContext = _contextFactory.Create();
        }

        public IDbSet<TEntity> CreateDbSet<TEntity>() where TEntity : class
        {
            return DbContext.Set<TEntity>();
        }

        public int SaveChanges()
        {
            return DbContext.SaveChanges();
        }
        
        public void Dispose()
        {
            DbContext.Dispose();
        }
    }


    /// <summary>
    /// Extension class for keeping instances of object stored in the current OperationContext.
    /// </summary>
    public class OperationContextStorageExtension : IExtension<OperationContext>
    {
        class ValueAndAction
        {
            public ValueAndAction(object value, Action action)
            {
                Value = value;
                Action = action;
            }

            public object Value { get; private set; }
            public Action Action { get; private set; }
        }
        private readonly IDictionary<object, ValueAndAction> _instances;

        private OperationContextStorageExtension()
        {
            _instances = new Dictionary<object, ValueAndAction>();
        }

        /// <summary>
        /// Store an object
        /// </summary>
        /// <param name="value">Object to store</param>
        /// <param name="completedAction">Optional Action to invoke when the current operation has completed.</param>
        public void Store<T>(T value, Action completedAction = null)
        {
            _instances[typeof(T)] = new ValueAndAction(value, completedAction);
        }

        /// <summary>
        /// Store an object
        /// </summary>
        /// <param name="value">Object to store</param>
        /// <param name="key">Key to use</param>
        /// <param name="completedAction">Optional Action to invoke when the current operation has completed.</param>
        public void Store<T>(T value, object key, Action completedAction = null)
        {
            _instances[key] = new ValueAndAction(value, completedAction);
        }

        /// <summary>
        /// Get a stored object
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The stored object or null if no object of type <typeparamref name="T"/> has been stored</returns>
        public T Get<T>()
        {
            ValueAndAction obj;
            if (!_instances.TryGetValue(typeof(T), out obj))
                return default(T);

            return (T)obj.Value;
        }

        /// <summary>
        /// Get a stored object
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="key">Key of the object</param>
        /// <returns>The stored object or null if no object with that key has been stored</returns>
        public T Get<T>(object key)
        {
            ValueAndAction obj;
            if (!_instances.TryGetValue(key, out obj))
                return default(T);

            return (T)obj.Value;
        }

        public void Attach(OperationContext owner)
        {
            // Make sure we are notified when the operation is complete
            owner.OperationCompleted += OnOperationCompleted;
        }

        public void Detach(OperationContext owner)
        {
            owner.OperationCompleted -= OnOperationCompleted;
        }

        void OnOperationCompleted(object sender, EventArgs e)
        {
            // Invoke any actions
            foreach (var obj in _instances.Values)
            {
                if (obj.Action != null)
                    obj.Action.Invoke();
            }
        }

        /// <summary>
        /// Get or create the one and only StorageExtension
        /// </summary>
        public static OperationContextStorageExtension Current
        {
            get
            {
                var opContext = OperationContext.Current;
                if (opContext == null)
                    throw new InvalidOperationException("No OperationContext");

                var currentContext = opContext.Extensions.Find<OperationContextStorageExtension>();
                if (currentContext == null)
                {
                    currentContext = new OperationContextStorageExtension();
                    opContext.Extensions.Add(currentContext);
                }

                return currentContext;
            }
        }
    }

    /// <summary>
    /// Base class for implementing the Repository pattern over an Entity Framework entity
    /// </summary>
    /// <remarks>Usually you will derive from this class to create an entity-specific repository</remarks>
    /// <typeparam name="TEntity">Entity to expose via the Repository</typeparam>
    public class RepositoryBase<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly IDbSet<TEntity> _objectSet;

        public RepositoryBase(IDbSetProvider objectSetProvider)
        {
            _objectSet = objectSetProvider.CreateDbSet<TEntity>();

            if (_objectSet == null)
                throw new InvalidOperationException("Unable to create object set");
        }

        /// <summary>
        /// This property can be used by derived classes in order to get access to the underlying DbSet
        /// </summary>
        protected virtual IQueryable<TEntity> Query
        {
            get { return _objectSet; }
        }

        /// <summary>
        /// Return all entities
        /// </summary>
        public virtual IEnumerable<TEntity> List()
        {
            return Query.ToArray();
        }

        /// <summary>
        /// Return all entities that match the <c cref="whereClause">whereClause</c>
        /// </summary>
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> whereClause)
        {
            return Query.Where(whereClause).ToArray();
        }

        /// <summary>
        /// Return the first entity that matches the <c cref="whereClause">whereClause</c>
        /// </summary>
        public virtual TEntity Get(Expression<Func<TEntity, bool>> whereClause)
        {
            return Query.Where(whereClause).FirstOrDefault();
        }

        /// <summary>
        /// Add a new entity to the repository
        /// </summary>
        public virtual void Add(TEntity entity)
        {
            _objectSet.Add(entity);
        }

        /// <summary>
        /// Delete an existing entity from the repository
        /// </summary>
        public virtual void Delete(TEntity entity)
        {
            _objectSet.Remove(entity);
        }
    }

    public static class RepositoryExtensions
    {/*
		public static IQueryable<TSource> Include<TSource>(this IQueryable<TSource> source, string path)
		{
			var objectQuery = source as ObjectQuery<TSource>;
			if(objectQuery != null)
			{
				return objectQuery.Include(path);
			}
			return source;
		}*/
    }
}