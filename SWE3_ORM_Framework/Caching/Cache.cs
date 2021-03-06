using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SWE3_ORM_Framework.Caching
{
    public class Cache : ICache
    {
        private Dictionary<object, object> cache = new Dictionary<object, object>();
        private ICollection<object> tmpCache = new List<object>();
        private Dictionary<object, string> hashes = new Dictionary<object, string>();

        public object GetObject(object pk)
        {
            if (ContainsKey(pk))
            {
                return cache[pk];
            }
            return null;
        }

        public virtual void CacheObject(object obj)
        {
            if (obj != null)
            {
                cache[GetPK(obj)] = obj;
                hashes[GetPK(obj)] = GenerateHash(obj);
            }
        }

        public bool ContainsKey(object pk)
        {
            return cache.ContainsKey(pk);
        }

        public virtual void RemoveObject(object obj)
        {
            cache.Remove(GetPK(obj));
            hashes.Remove(GetPK(obj));
        }

        public virtual void ClearCache()
        {
            cache.Clear();
            hashes.Clear();
        }

        public virtual bool CacheChanged(object obj)
        {
            if (hashes.ContainsKey(GetPK(obj)))
            {
                return hashes[GetPK(obj)] != GenerateHash(obj);
            }

            return true;
        }

        /// <summary>
        /// Help method to get the primary key of an object by using ORMapper functionality.
        /// </summary>
        /// <param name="obj">Object that holds the values.</param>
        /// <returns>The primary key value of the object.</returns>
        protected object GetPK(object obj)
        {
            return ORMapper.GetTable(obj).PrimaryKey.GetObjectValue(obj);
        }

        public object SearchTmp(object pk)
        {
            foreach (object o in tmpCache)
            {
                if (GetPK(o).Equals(pk))
                {
                    return o;
                }
            }
            return null;
        }

        public void AddTmp(object obj)
        {
            tmpCache.Add(obj);
        }

        public void ClearTmp()
        {
            tmpCache.Clear();
        }

        /// <summary>
        /// Computes a hash by encoding an object with all its property values.
        /// One different property value will lead to a different hash.
        /// </summary>
        /// <param name="obj">Object to compute the hash for.</param>
        /// <returns>The string that contains the hash.</returns>
        private string GenerateHash(object obj)
        {
            string values = "";

            foreach (var col in ORMapper.GetTable(obj).TableCols)
            {
                var val = col.GetObjectValue(obj);

                if (val != null)
                {
                    if (col.IsFK)
                    {
                        values += GetPK(val).ToString() + ",";
                    }
                    else
                    {
                        values += val.ToString() + ",";
                    }
                }
            }

            foreach (var col in ORMapper.GetTable(obj).ReferencedCols)
            {
                var val = col.GetObjectValue(obj);

                if (val != null)
                {
                    foreach (var i in (IEnumerable)val)
                    {
                        values += GetPK(i).ToString() + ",";
                    }
                }
            }

            return Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(values)));
        }
    }
}
