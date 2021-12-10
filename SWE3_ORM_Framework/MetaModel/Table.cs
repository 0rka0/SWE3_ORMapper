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

        public Type Member { get; private set; }

        public string Name { get; private set; }

        public Column[] Columns { get; private set; }

        public Column[] TableCols { get; private set; }

        public Column[] ReferenceCols { get; private set; }

        public Column PrimaryKey { get; private set; }

        public string Discriminator { get; private set; }
    }
}
