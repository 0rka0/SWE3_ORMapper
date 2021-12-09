using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.MetaModel
{
    class Table
    {
        public Table(Type t)
        {

        }

        public string Name { get; set; }

        public Column[] Columns { get; set; }

        public Column PrimaryKey { get; set; }
    }
}
