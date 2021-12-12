using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.Exceptions
{
    public class OrmExceptionBase : ApplicationException
    {
        public string Method { get; }

        public OrmExceptionBase(string method, string message, Exception innerException) : base(message, innerException)
        {
            Method = method;
        }
    }
}
