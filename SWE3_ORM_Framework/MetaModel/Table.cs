using SWE3_ORM_Framework.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SWE3_ORM_Framework.MetaModel
{
    /// <summary>
    /// The MetaData for a table that depends on the class.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// The type of the class that the Table depends on.
        /// </summary>
        public Type Member { get; private set; }

        /// <summary>
        /// The name of the table. 
        /// Can be defined by setting a TableAttribute. Default name is the class name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// List of all columns that represent the properties of the class.
        /// </summary>
        public Column[] Columns { get; private set; }

        /// <summary>
        /// List of all columns that are directly part of the table and persisted in the database.
        /// </summary>
        public Column[] TableCols { get; private set; }

        /// <summary>
        /// List of all columns that are not part of and persisted in the table.
        /// Referenced columns are for object that reference this table and will be selected from the corresponding tables.
        /// </summary>
        public Column[] ReferencedCols { get; private set; }

        /// <summary>
        /// The primary key column of this table.
        /// </summary>
        public Column PrimaryKey { get; private set; }

        /// <summary>
        /// The discriminator of this table that is used to differenciate between classes in the database in the single table structure.
        /// If the discriminator is enabled it will be defined by the class name. 
        /// </summary>
        public string Discriminator { get; private set; } = null;

        /// <summary>
        /// The constructor of the table builds the object by using the characteristics of the class properties.
        /// </summary>
        /// <param name="t">The type of the class that the table will be created for.</param>
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
                    col.Nullable = false;
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
