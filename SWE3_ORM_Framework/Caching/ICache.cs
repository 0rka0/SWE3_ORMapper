using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.Caching
{
    public interface ICache
    {
        object GetObject(object pk);

        void CacheObject(object obj);

        void RemoveObject(object obj);

        bool ContainsKey(object pk);

        bool CacheChanged(object obj);

        public object SearchTmp(object obj);

        public void AddTmp(object obj);

        public void ClearTmp();
    }
}
