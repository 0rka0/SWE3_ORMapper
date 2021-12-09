using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.Attributes
{
    public class ForeignKey : ColumnAttribute
    {
        public string TargetTable = null;

        public string TargetColumn = null;
    }
}
