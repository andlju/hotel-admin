using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Petite
{
	/// <summary>
	/// Interface for the Context (UnitOfWork)
	/// </summary>
	public interface IObjectContext
	{
		/// <summary>
		/// Save all pending changes 
		/// </summary>
		/// <returns>Number of items that had pending changes</returns>
		int SaveChanges();
	}

	/// <summary>
	/// Interface for all repositories
	/// </summary>
	/// <typeparam name="TEntity">Aggregate root type for this repository</typeparam>
	public interface IRepository<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Get a single entity
		/// </summary>
		/// <param name="whereClause">Selection expression</param>
		/// <returns>The first entity that matches the <paramref name="whereClause"/></returns>
		TEntity Get(Expression<Func<TEntity, bool>> whereClause);
		
		/// <summary>
		/// List all entities
		/// </summary>
		/// <returns>A list of all entities</returns>
		IEnumerable<TEntity> List();

		/// <summary>
		/// Find all entities that matches a selection
		/// </summary>
		/// <param name="whereClause">Selection expression</param>
		/// <returns>A list of all entities that match the <paramref name="whereClause"/></returns>
		IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> whereClause);

		/// <summary>
		/// Add an entity to the repository
		/// </summary>
		/// <param name="entity">Entity to add</param>
		void Add(TEntity entity);

		/// <summary>
		/// Remove an entity from the repostitory
		/// </summary>
		/// <param name="entity">Entity to remove</param>
		void Delete(TEntity entity);
	}

}