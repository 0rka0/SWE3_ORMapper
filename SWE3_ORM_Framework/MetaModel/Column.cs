using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.MetaModel
{
    class Column
    {
        public Table Table { get; private set; }

        public MemberInfo TableMember { get; set; }

        public Type TableType 
        { 
            get
            {
                if (TableMember is PropertyInfo)
                    return ((PropertyInfo)TableMember).PropertyType;
                throw new NotSupportedException("Table type is not supported");
            }    
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsPK { get; set; } = false;

        public bool IsFK { get; set; } = false;

        public string TargetTable { get; set; }

        public string TargetColumn { get; set; }

        public bool Nullable { get; set; } = true;

        public bool IsReferenceCol { get; set; } = false;

        public bool IsMtoN { get; set; }

        public Column(Table table)
        {
            Table = table;
        }
    }
}
