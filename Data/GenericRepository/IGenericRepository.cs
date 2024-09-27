using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Data.GenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type T asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains an enumerable collection of entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be retrieved.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the entity, or null if not found.</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new entity of type T in the data store asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be created.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Deletes an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be deleted.</param>
        Task DeleteByIdAsync(Guid id);

        /// <summary>
        /// Saves any pending changes to the database asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains true if the save was successful; otherwise, false.</returns>
        Task<bool> SaveChangesAsync();
    }

}