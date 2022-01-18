using SWE3_ORM_Framework.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SWE3_ORM_Framework.MetaModel
{
    /// <summary>
    /// The MetaData for a column that depends on a property of a class.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// The table of the class the column is a property of.
        /// </summary>
        public Table Table { get; private set; }

        /// <summary>
        /// Information of the property.
        /// </summary>
        public PropertyInfo Member { get; set; }

        /// <summary>
        /// Type of the property.
        /// </summary>
        public Type MemberType 
        { 
            get
            {
                return Member.PropertyType;
            }    
        }

        /// <summary>
        /// Name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type defined by the column attribute which will be used in the database.
        /// Default type will be the type of the property if nothing else has been defined.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Defines if the property is the primary key of the table.
        /// </summary>
        public bool IsPK { get; set; } = false;

        /// <summary>
        /// Defines if the property is a foreign key that references or is referenced by another table.
        /// </summary>
        public bool IsFK { get; set; } = false;

        /// <summary>
        /// Defines the target table that will be used as a middle table for MtoN relationships.
        /// </summary>
        public string TargetTable { get; set; }

        /// <summary>
        /// The column of the primary key of the related object that will be set in the middle table.
        /// </summary>
        public string TargetColumn { get; set; }

        /// <summary>
        /// Defines if the property will be nullable in the database.
        /// </summary>
        public bool Nullable { get; set; } = true;

        /// <summary>
        /// Defines if the column is direct part of the table or a referenced column that is part of another table.
        /// </summary>
        public bool IsReferencedCol { get; set; } = false;

        /// <summary>
        /// Defines if the column is a part of a MtoN relationship.
        /// </summary>
        public bool IsMtoN { get; set; }

        /// <summary>
        /// The constructor of the column builds the object and sets the table property.
        /// </summary>
        /// <param name="table">The table the column is a part of.</param>
        public Column(Table table)
        {
            Table = table;
        }

        /// <summary>
        /// Gets value from an object by getting the value of the property that is allocated to this column.
        /// </summary>
        /// <param name="obj">The object that contains the values.</param>
        /// <returns>The value of this column.</returns>
        public object GetObjectValue(object obj)
        {
            var value = Member.GetValue(obj);

            return value;
        }

        /// <summary>
        /// Sets the value of the property of an object, that this column is allocated to.
        /// </summary>
        /// <param name="obj">The object which values will be changed.</param>
        /// <param name="value">The value that is assigned to the specified property.</param>
        public void SetObjectValue(object obj, object value)
        {
            Member.SetValue(obj, value);
            return;
        }

        /// <summary>
        /// Changes the code type of an object to the corresponding database type if necessary.
        /// Type will be changed if change is necessary or if a different type was specified by using the column attribute.
        /// </summary>
        /// <param name="obj">The object that will be checked for adjustment.</param>
        /// <returns>Object with new type that fits the database.</returns>
        public object ToColumnType(object obj)
        {
            if (IsFK)
            {
                if (obj == null)
                    return null;

                return ORMapper.GetTable(MemberType).PrimaryKey.ToColumnType(ORMapper.GetTable(MemberType).PrimaryKey.GetObjectValue(obj));
            }

            if (obj is Enum)
            {
                if (Type == typeof(short))
                {
                    return (short)obj;
                }
                if (Type == typeof(long))
                {
                    return (long)obj;
                }
                return (int)obj;
            }

            if (MemberType == Type)
                return obj;

            if (obj is bool)
            {
                if(Type == typeof(int))
                {
                    if ((bool)obj)
                        return 1;
                    return 0;
                }
                if (Type == typeof(short))
                {
                    if ((bool)obj)
                        return (short)1;
                    return (short)0;
                }
                if (Type == typeof(long))
                {
                    if ((bool)obj)
                        return (long)1;
                    return (long)0;
                }
            }

            return obj;
        }

        /// <summary>
        /// Changes the database type of an object to the corresponding code type if necessary.
        /// If the column is a foreign key, it will be changed to the referenced object.
        /// </summary>
        /// <param name="obj">The object that will be checked for adjustment.</param>
        /// <returns>Object with new type that fits the code.</returns>
        public object ToCodeType(object obj)
        {
            if (IsFK)
            {
                return ORMapper.GetByPK(obj, MemberType);
            }

            if (MemberType == typeof(bool))
            {
                if (obj is int)
                    return (int)obj != 0;
                if (obj is short)
                    return (short)obj != 0;
                if (obj is long)
                    return (long)obj != 0;
            }

            if (MemberType == typeof(short))
                return (short)obj;
            if (MemberType == typeof(int))
                return (int)obj;
            if (MemberType == typeof(long))
                return (long)obj;

            if (Type.IsEnum)
                return Enum.ToObject(Type, (int)obj);

            return obj;
        }

        /// <summary>
        /// Fills the data that was included from the referenced table.
        /// </summary>
        /// <param name="list">The list that will be filled with the read data.</param>
        /// <param name="obj">The object itself which primary key will be used to get referenced data from another table.</param>
        /// <param name="type">The type of the object given.</param>
        /// <returns>The list with included data from a referenced table.</returns>
        public object FillReferencedColumn(object list, object obj, Type type)
        {
            var dict = new Dictionary<string, object>() { { ":fk", Table.PrimaryKey.GetObjectValue(obj) } };
            ORMapper.FillList(MemberType.GenericTypeArguments[0], list, GetReferenceSql(), dict );        
            return list;
        }

        /// <summary>
        /// Creates the sql that is used to select all references for the column type that fit the foreign key.
        /// </summary>
        /// <returns>The sql string to select the references.</returns>
        public string GetReferenceSql()
        {
            Table table = ORMapper.GetTable(MemberType.GenericTypeArguments[0]);
            string sql = $"SELECT * FROM {table.Name} WHERE ";

            if (table.Discriminator != null)
            {
                sql += ORMapper.GetDiscriminatorSql(table.Member);
            }

            if (IsMtoN)
            {
                return sql += $"ID IN (SELECT {TargetColumn} FROM {TargetTable} WHERE {Name} = :fk)";
            }

            return sql += $"{Name} = :fk";
        }

        /// <summary>
        /// Sets the references at the middle table in MtoN relationships.
        /// </summary>
        /// <param name="obj">The currect object which references will be set.</param>
        public void SetReferences(object obj)
        {
            var refType = Type.GetGenericArguments()[0];
            var refTable = ORMapper.GetTable(refType);
            var primaryKey = Table.PrimaryKey.ToColumnType(Table.PrimaryKey.GetObjectValue(obj));

            if (IsMtoN)
            {
                ORMapper.PrepTargetTable(TargetTable, Name, primaryKey);

                foreach (object o in (IEnumerable)GetObjectValue(obj))
                {
                    ORMapper.InsertIntoMiddleTable(o, TargetTable, Name, primaryKey, TargetColumn, refTable);
                }
            }
        }
    }
}
