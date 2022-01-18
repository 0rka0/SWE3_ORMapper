using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.Caching
{
    /// <summary>
    /// Cache that temporarily persists objects during runtime.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Gets an object from the cache by using the primary key as an identifier.
        /// </summary>
        /// <param name="pk">The primary key used to identify the object.</param>
        /// <returns>The object if one was found, otherwise null.</returns>
        object GetObject(object pk);

        /// <summary>
        /// Puts an object into the normal and the hashes cache to persist during runtime.
        /// </summary>
        /// <param name="obj">Object to be persisted.</param>
        void CacheObject(object obj);

        /// <summary>
        /// Removes object from the normal and the hashes cache if it is no longer needed.
        /// </summary>
        /// <param name="obj">The object to be removed.</param>
        void RemoveObject(object obj);

        /// <summary>
        /// Removes all objects from the normal and the hashes cache.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Checks if the specified primary key is found in the cache.
        /// </summary>
        /// <param name="pk">Primary key to be searched.</param>
        /// <returns>True if the cache contains the primary key, otherwise false.</returns>
        bool ContainsKey(object pk);

        /// <summary>
        /// Checks if the object has changed since it was put into the cache by comparing the hash of the object to the one in the hashes cache. 
        /// </summary>
        /// <param name="obj">The object to be checked.</param>
        /// <returns>True if any value of the object has changed, otherwise false.</returns>
        bool CacheChanged(object obj);

        /// <summary>
        /// Searches the temporary cache for a specific object by using the primary key as an identifier.
        /// The temporary cache only contains the data for a specific time period when it is needed. 
        /// </summary>
        /// <param name="obj">The primary key used to identify the object.</param>
        /// <returns>The object if one was found, otherwise null.</returns>
        public object SearchTmp(object pk);

        /// <summary>
        /// Puts an object into the temporary cache to persist data for a specific time period when it is needed.
        /// </summary>
        /// <param name="obj">Object to be temporarily persisted.</param>
        public void AddTmp(object obj);

        /// <summary>
        /// Used to completely clear the temporary cache if it is no longer needed.
        /// </summary>
        public void ClearTmp();
    }
}
