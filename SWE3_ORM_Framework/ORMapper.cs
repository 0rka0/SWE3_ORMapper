using SWE3_ORM_Framework.Caching;
using SWE3_ORM_Framework.Exceptions;
using SWE3_ORM_Framework.MetaModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SWE3_ORM_Framework
{
    /// <summary>
    /// General OR-Mapper class. Functions can be called to perform basic Database interactions such as:
    /// Create: Inserts a new object into the database.
    /// Update: Updates existing entry in database.
    /// Get: Returns a single object from the database.
    /// Remove: Removes single entry from the database.
    /// </summary>
    public static class ORMapper
    {
        /// <summary>
        /// Holds the connection to the database during runtime.
        /// </summary>
        private static IDbConnection Connection { get; set; }

        /// <summary>
        /// Caches objects during runtime to reduce the load on the database and enable the possibility of including references.
        /// </summary>
        private static ICache cache = new Cache();

        /// <summary>
        /// Starts connection to the database with the given parameter.
        /// </summary>
        /// <param name="connection">Connection created with connection string.</param>
        public static void StartConnection(IDbConnection connection)
        {
            Connection = connection;
            Connection.Open();
        }

        /// <summary>
        /// Gets a single object with a specific type and primary key from the database if one was found during the selecting process.
        /// </summary>
        /// <param name="primaryKey">Primary key of the object that wants to be selected.</param>
        /// <param name="type">Type of the object that wats to be selected. Takes typeof(class).</param>
        /// <returns>Returns the selected object. Otherwise retrns null.</returns>
        public static object GetByPK(object primaryKey, Type type)
        {
            if(cache.ContainsKey(primaryKey))
            {
                return cache.GetObject(primaryKey);
            }

            object value = null;
            Table table = GetTable(type);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {table.Name} WHERE ";

            if (table.Discriminator != null)
            {
                cmd.CommandText += GetDiscriminatorSql(type);
            }

            IDataParameter pkParam = cmd.CreateParameter();
            cmd.CommandText += $"{table.PrimaryKey.Name} = :v1";
            pkParam.ParameterName = ":v1";
            pkParam.Value = primaryKey;
            cmd.Parameters.Add(pkParam);

            IDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var values = TransformReader(reader);
                reader.Close();
                value = CreateObject(type, values);
            }

            reader.Dispose();
            cmd.Dispose();

            cache.CacheObject(value);
            cache.ClearTmp();

            return value;
        }

        /// <summary>
        /// Selects data from database by using a custom specific parameters, the sql will be auto generated.
        /// </summary>
        /// <typeparam name="T">The type of the selected objects.</typeparam>
        /// <param name="parameters">The parameters to fill the sql query.</param>
        /// <param name="sqlOperator">Defines which operator the sql uses. True for AND and False for OR. Default will be AND.</param>
        /// <returns>A List consisting of all the objects that were selected.</returns>
        public static List<T> GetByParams<T>(List<Tuple<string, object>> parameters, bool sqlOperator = true)
        {
            if (parameters == null || parameters.Count == 0)
                return null;

            string disc;
            if (sqlOperator)
                disc = " AND ";
            else
                disc = " OR ";

            Type type = typeof(T);
            Table table = GetTable(typeof(T));
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            List<T> res = new List<T>();

            var sql = $"SELECT * FROM {table.Name} WHERE ";

            if (table.Discriminator != null)
            {
                sql += GetDiscriminatorSql(type);
            }

            sql += "(";
            for (int i = 0; i < parameters.Count; i++)
            {
                if (i > 0)
                    sql += disc;

                var pName = ":v" + i;
                sql += $"{parameters[i].Item1} = {pName}";

                sqlParams.Add(pName, parameters[i].Item2);  
            }
            sql += ")";

            FillList(type, res, sql, sqlParams);

            return res;
        }

        /// <summary>
        /// Selects data from database by using a custom sql.
        /// </summary>
        /// <typeparam name="T">The type of the selected objects.</typeparam>
        /// <param name="sql">The sql used to select from the database.</param>
        /// <param name="parameters">The parameters to fill the sql query.</param>
        /// <returns>A List consisting of all the objects that were selected.</returns>
        public static List<T> GetBySql<T>(string sql, Dictionary<string, object> parameters)
        {
            Type type = typeof(T);
            List<T> res = new List<T>();

            FillList(type, res, sql, parameters);

            return res;
        }

        /// <summary>
        /// Takes a type and searches its assembly for subtypes that derive from the specified type.
        /// </summary>
        /// <param name="type">The type that subtypes will be searched for.</param>
        /// <returns>Array of the subtypes if any were found.</returns>
        public static Type[] GetSubtypes(Type type)
        {
            var tmpSubtypes = Assembly
                    .GetAssembly(type)
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(type));
            return tmpSubtypes.ToArray();
        }

        /// <summary>
        /// Creates the sql string addition to select objects of a certain type in a single table that consists of multiple types by their discriminator.
        /// </summary>
        /// <param name="type">The type to determine which table and discriminator will be used.</param>
        /// <returns>The string with the corresponding sql.</returns>
        public static string GetDiscriminatorSql(Type type)
        {
            Table table = GetTable(type);
            var sql = "";
            var subtypes = GetSubtypes(type);

            sql += $"({nameof(table.Discriminator)} = '{table.Discriminator}'";

            for (int i = 0; i < subtypes.Length; i++)
            {
                sql += $" OR {nameof(table.Discriminator)} = '{subtypes[i].Name}'";
            }

            sql += ") AND ";
            return sql;
        }

        /// <summary>
        /// Returns a Dictionary with the values of the Datareader for a single row so the reader can be closed and reused before the read data is processed.
        /// Alternative to deal with multiple result sets.
        /// </summary>
        /// <param name="reader">The Datareader that data was read with.</param>
        /// <returns>A dictionary with the read data. The key represents the column name and the value holds an object with the database entry.</returns>
        public static Dictionary<string, object> TransformReader(IDataReader reader)
        {
            return Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);
        }

        /// <summary>
        /// Inserts an object into the database.
        /// </summary>
        /// <param name="obj">Object that will be inserted into the database</param>
        public static void Create(object obj)
        {
            Table table = GetTable(obj);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = "INSERT INTO " + table.Name;

            IDataParameter param;

            string pName;
            string cols = "";
            string values = "";

            for (int i = 0; i < table.TableCols.Length; i++)
            {
                if (i > 0)
                {
                    cols += ", ";
                    values += ", ";
                }

                pName = ":v" + i;
                cols += table.TableCols[i].Name;
                values += pName;

                param = cmd.CreateParameter();
                param.ParameterName = pName;
                param.Value = table.TableCols[i].ToColumnType(table.TableCols[i].GetObjectValue(obj));
                cmd.Parameters.Add(param);
            }
            
            if (!string.IsNullOrWhiteSpace(table.Discriminator))
            {
                cols += ", " + nameof(table.Discriminator);
                pName = ":v" + table.TableCols.Length;
                values += ", " + pName;
                param = cmd.CreateParameter();
                param.ParameterName = pName;
                param.Value = table.Discriminator;
                cmd.Parameters.Add(param);
            }

            cmd.CommandText += $"({cols}) VALUES ({values})";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new OrmDbException(nameof(Create), "Error occured while accessing database, check inner exception.", ex);
            }
            cmd.Dispose();

            SetReferences(table, obj);

            cache.CacheObject(obj);
        }

        /// <summary>
        /// Updates exisiting object in the database and overwrites existing values with the new values.
        /// </summary>
        /// <param name="obj">Object with new values that will replace the object with the same primary key.</param>
        public static void Update(object obj)
        {
            if (!cache.CacheChanged(obj))
            {
                return;
            }

            Table table = GetTable(obj);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = "UPDATE " + table.Name + " SET ";

            IDataParameter param;

            string pairs = "";
            string pName;
            string pk = "";

            for (int i = 0; i < table.TableCols.Length; i++)
            {
                if (i > 0)
                    pairs += ", ";

                pName = ":v" + i;
                pairs += table.TableCols[i].Name + " = " + pName;
                param = cmd.CreateParameter();
                param.ParameterName = pName;
                param.Value = table.TableCols[i].ToColumnType(table.TableCols[i].GetObjectValue(obj));
                cmd.Parameters.Add(param);

                if(table.TableCols[i].IsPK)
                    pk = pName;
            }

            cmd.CommandText += $"{pairs} WHERE {table.PrimaryKey.Name} = {pk}";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new OrmDbException(nameof(Create), "Error occured while accessing database, check inner exception.", ex);
            }
            cmd.Dispose();

            SetReferences(table, obj);

            cache.CacheObject(obj);
        }

        /// <summary>
        /// Removes object from the database.
        /// </summary>
        /// <param name="obj">Object that will be removed from the database by primary key.</param>
        public static void Remove(object obj)
        {
            Table table = GetTable(obj);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {table.Name} WHERE {table.PrimaryKey.Name} = :v1";
            
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ":v1";
            param.Value = table.PrimaryKey.GetObjectValue(obj);
            cmd.Parameters.Add(param);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new OrmDbException(nameof(Create), "Error occured while accessing database, check inner exception.", ex);
            }
            cmd.Dispose();

            cache.RemoveObject(obj);
        }

        /// <summary>
        /// Creates and returns a MetaModel Table object for a given type.
        /// Type will either be the parameter itself or taken from the given object.
        /// </summary>
        /// <param name="obj">Object or type that the Table will be created with.</param>
        /// <returns>The Table that has been created using the parameter.</returns>
        public static Table GetTable(object obj)
        {
            if (obj is Type)
                return new Table((Type)obj);
            return new Table(obj.GetType());
        }

        /// <summary>
        /// Creates a complete object from its type by using the reader values.
        /// </summary>
        /// <param name="type">The type the created object will be an instance of.</param>
        /// <param name="reader">Contains the values for each property read by the Datareader in dictionary format.</param>
        /// <returns>The object with the properties from the reader.</returns>
        public static object CreateObject(Type type, Dictionary<string,object> reader)
        {
            if(reader.ContainsKey("discriminator"))
            {
                var classType = reader["discriminator"];
                var asm = type.Assembly;
                type = asm.GetTypes().Single(t => t.Name == classType.ToString());
            }

            var table = GetTable(type);

            var pk = table.PrimaryKey.ToCodeType(reader[table.PrimaryKey.Name.ToLower()]);

            object obj = null;
            
            if(cache.ContainsKey(pk))
            {
                obj = cache.GetObject(pk);
            }
            else
            {
                obj = cache.SearchTmp(pk);
            }

            if (obj == null)
            {
                obj = Activator.CreateInstance(type);
                cache.AddTmp(obj);
            }
            else
                return obj;

            foreach (var col in table.TableCols)
            {
                col.SetObjectValue(obj, col.ToCodeType(reader[col.Name.ToLower()]));
            }

            foreach (var col in table.ReferencedCols)
            {
                col.SetObjectValue(obj, col.FillReferencedColumn(Activator.CreateInstance(col.MemberType), obj, type));
            }

            return obj;
        }

        /// <summary>
        /// Reads the data of specified parameters from database and includes the data into a list.
        /// </summary>
        /// <param name="type">The type of the foreign key that the referenced data will be of.</param>
        /// <param name="list">The list of objects that the foreign key references.</param>
        /// <param name="sql">The sql that is used to select all references for the type of the foreign key.</param>
        /// <param name="parameters">The parameters that will be added to the sql.</param>
        public static void FillList(Type type, object list, string sql, Dictionary<string, object> parameters)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = sql;

            foreach(var pair in parameters)
            {
                IDataParameter param = cmd.CreateParameter();
                param.ParameterName = pair.Key;
                param.Value = pair.Value;
                cmd.Parameters.Add(param);
            }

            IDataReader reader = cmd.ExecuteReader();
            List<Dictionary<string, object>> values = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                values.Add(TransformReader(reader));
            }
            reader.Close();

            foreach (var dict in values)
            {
                var value = CreateObject(type, dict);
                list.GetType().GetMethod("Add").Invoke(list, new object[] { value });
            }

            reader.Dispose();
            cmd.Dispose();
        }

        /// <summary>
        /// Sets the references for each reference column of an object after it is created or changed.
        /// </summary>
        /// <param name="table">The Table object that defines the affiliation of the properties.</param>
        /// <param name="obj">The object which referenced properties will be set.</param>
        private static void SetReferences(Table table, object obj)
        {
            foreach(var col in table.ReferencedCols)
            {
                col.SetReferences(obj);
            }
        }

        /// <summary>
        /// Clears the target table of all entries with set primary key that might conflict with the new data.
        /// </summary>
        /// <param name="targetTable">The table which will be affected by the changes.</param>
        /// <param name="colName">The column of the primary key.</param>
        /// <param name="pk">The value of the primary key.</param>
        public static void PrepTargetTable(string targetTable, string colName, object pk)
        {
            string pName = ":pk";
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {targetTable} WHERE {colName} = {pName}";

            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = pName;
            param.Value = pk;
            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Inserts data into a middle table while persisting MtoN relationships.
        /// </summary>
        /// <param name="obj">The object containing the values.</param>
        /// <param name="targetTable">The middle table that the data will be inserted into.</param>
        /// <param name="colName">The column of the primary key of the object that will be set in the middle table.</param>
        /// <param name="pk">The value of the primary key.</param>
        /// <param name="targetColName">The column of the primary key of the related object that will be set in the middle table.</param>
        /// <param name="refTable">The table object of the related object.</param>
        public static void InsertIntoMiddleTable(object obj, string targetTable, string colName, object pk, string targetColName, Table refTable)
        {
            string pName = ":pk";
            string p2Name = ":fk";
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"INSERT INTO {targetTable} ({colName}, {targetColName}) VALUES ({pName}, {p2Name})";

            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = pName;
            param.Value = pk;
            cmd.Parameters.Add(param);

            param = cmd.CreateParameter();
            param.ParameterName = p2Name;
            param.Value = refTable.PrimaryKey.ToColumnType(refTable.PrimaryKey.GetObjectValue(obj));
            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Deletes all entries from all tables in the database.
        /// </summary>
        public static void ResetDatabase()
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = "TRUNCATE persons, classes, courses, student_courses CASCADE";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes all entries from the cache.
        /// </summary>
        public static void ClearCache()
        {
            cache.ClearCache();
        }
    }
}
