using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.Exceptions
{
    public class OrmDbException : OrmExceptionBase
    {
        public OrmDbException(string method, string message, Exception innerException) : base(method, message, innerException) 
        { 
        }
    }
}
