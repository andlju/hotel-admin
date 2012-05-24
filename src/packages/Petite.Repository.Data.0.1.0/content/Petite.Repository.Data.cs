using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;

namespace Petite
{
	/// <summary>
	/// Interface implemented by the Context class to provide an Object Set
	/// </summary>
	public interface IObjectSetProvider
	{
		/// <summary>
		/// Create an Object Set for a specific entity
		/// </summary>
		/// <typeparam name="TEntity">Type of the entity to provide a set for</typeparam>
		/// <returns>An object set for <typeparamref name="TEntity"/> objects</returns>
		IObjectSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class;
	}

	/// <summary>
	/// Interface for creating an object context
	/// </summary>
	public interface IObjectContextFactory
	{
		/// <summary>
		/// Create an Entity Framework object context
		/// </summary>
		/// <returns>A new ObjectContext</returns>
		ObjectContext Create();

		/// <summary>
		/// Identifies this Object Context
		/// </summary>
		string ContextTypeName { get; }
	}

	/// <summary>
	/// Factory for creating an Entity Framework ObjectContext
	/// </summary>
	/// <typeparam name="TContext">Type of ObjectContext to create</typeparam>
	public class ObjectContextFactory<TContext> : IObjectContextFactory
		where TContext : ObjectContext, new()
	{
		public ObjectContext Create()
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
	public class WcfObjectContextAdapter : IObjectSetProvider, IObjectContext
	{
		private readonly IObjectContextFactory _contextFactory;

		/// <summary>
		/// Create a new instance of the adapter
		/// </summary>
		/// <param name="contextFactory">Factory to use to construct the DbContext</param>
		public WcfObjectContextAdapter(IObjectContextFactory contextFactory)
		{
			_contextFactory = contextFactory;
		}

		protected virtual string KeyName { get { return _contextFactory.ContextTypeName; } }

		public IObjectSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class
		{
			var context = GetCurrentContext();
			return context.CreateObjectSet<TEntity>();
		}

		public int SaveChanges()
		{
			var context = GetCurrentContext();
			return context.SaveChanges();
		}

		public void Detach(object obj)
		{
			var context = GetCurrentContext();
			context.Detach(obj);
		}

		protected ObjectContext GetCurrentContext()
		{
			var storageExtension = OperationContextStorageExtension.Current;

			var objContext = storageExtension.Get<ObjectContext>(KeyName);

			if(objContext == null)
			{
				// No DbContext has been created for this operation yet. Create a new one...
				objContext = _contextFactory.Create();
				// ... store it and make sure it will be Disposed when the operation is completed.
				storageExtension.Store(objContext, KeyName, () => objContext.Dispose());
			}

			return objContext;
		}
	}

	/// <summary>
	/// A simple implementation of IObjectContext and IObjectSetProvider that creates a single object context
	/// and uses it
	/// </summary>
	public class SingleUsageObjectContextAdapter : IObjectSetProvider, IObjectContext
	{
		private readonly IObjectContextFactory _contextFactory;
		protected ObjectContext ObjContext { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextFactory"></param>
		public SingleUsageObjectContextAdapter(IObjectContextFactory contextFactory)
		{
			_contextFactory = contextFactory;
			ObjContext = _contextFactory.Create();
		}

		public IObjectSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class
		{
			return ObjContext.CreateObjectSet<TEntity>();
		}

		public int SaveChanges()
		{
			return ObjContext.SaveChanges();
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
			if(!_instances.TryGetValue(typeof(T), out obj))
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
			if(!_instances.TryGetValue(key, out obj))
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
			foreach(var obj in _instances.Values)
			{
				if(obj.Action != null)
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
				if(opContext == null)
					throw new InvalidOperationException("No OperationContext");

				var currentContext = opContext.Extensions.Find<OperationContextStorageExtension>();
				if(currentContext == null)
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
		private readonly IObjectSet<TEntity> _objectSet;

		public RepositoryBase(IObjectSetProvider objectSetProvider)
		{
			_objectSet = objectSetProvider.CreateObjectSet<TEntity>();

			if(_objectSet == null)
				throw new InvalidOperationException("Unable to create object set");
		}

		/// <summary>
		/// This property can be used by derived classes in order to get access to the underlying DbSet
		/// </summary>
		protected virtual IObjectSet<TEntity> Query
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
			_objectSet.AddObject(entity);
		}

		/// <summary>
		/// Delete an existing entity from the repository
		/// </summary>
		public virtual void Delete(TEntity entity)
		{
			_objectSet.DeleteObject(entity);
		}
	}

	public static class RepositoryExtensions
	{
		public static IQueryable<TSource> Include<TSource>(this IQueryable<TSource> source, string path)
		{
			var objectQuery = source as ObjectQuery<TSource>;
			if(objectQuery != null)
			{
				return objectQuery.Include(path);
			}
			return source;
		}
	}
}