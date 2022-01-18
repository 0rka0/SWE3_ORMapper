using SWE3_ORM_Framework.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SWE3_ORM_Framework.MetaModel
{
    public class Table
    {
        public Type Member { get; private set; }

        public string Name { get; private set; }

        public Column[] Columns { get; private set; }

        public Column[] TableCols { get; private set; }

        public Column[] ReferencedCols { get; private set; }

        public Column PrimaryKey { get; private set; }

        public string Discriminator { get; private set; } = null;

        public Table(Type t)
        {
            var tableAttribute = (TableAttribute)t.GetCustomAttribute(typeof(TableAttribute));

            Name = tableAttribute.Name;
            if (String.IsNullOrWhiteSpace(Name))
                Name = t.Name.ToLower();
            
            if(tableAttribute.Discriminator)
                Discriminator = t.Name.ToString();

            Member = t;

            List<Column> cols = new List<Column>();

            foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (prop.GetCustomAttribute(typeof(Ignore)) != null) 
                    continue;

                var col = new Column(this);
                var colAttribute = (ColumnAttribute)prop.GetCustomAttribute(typeof(ColumnAttribute), true);

                if (colAttribute == null)
                {
                    if (prop.GetGetMethod() == null || !prop.GetGetMethod().IsPublic)
                        continue;
                }

                if (colAttribute != null)
                {
                    col.Name = colAttribute.Name;
                    col.Type = colAttribute.Type;
                    col.Nullable = colAttribute.Nullable;
                }

                col.Member = prop;

                if (String.IsNullOrWhiteSpace(col.Name))
                    col.Name = prop.Name;
                if (col.Type == null)
                    col.Type = prop.PropertyType;

                if (colAttribute is PrimaryKey)
                {
                    PrimaryKey = col;
                    col.IsPK = true;
                }

                if(colAttribute is ForeignKey)
                {
                    col.IsFK = true;

                    col.IsReferencedCol = typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);

                    col.TargetTable = ((ForeignKey)colAttribute).TargetTable;
                    col.TargetColumn = ((ForeignKey)colAttribute).TargetColumn;
                    col.IsMtoN = !String.IsNullOrWhiteSpace(col.TargetTable);
                }

                cols.Add(col);
            }

            Columns = cols.ToArray();
            TableCols = cols.Where(c => !c.IsReferencedCol).ToArray();
            ReferencedCols = cols.Where(c => c.IsReferencedCol).ToArray();
        }
    }
}
