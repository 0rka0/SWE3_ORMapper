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
    public static class ORMapper
    {
        private static IDbConnection Connection { get; set; }

        private static ICache cache = new Cache();

        public static void StartConnection(IDbConnection connection)
        {
            Connection = connection;
            Connection.Open();
        }

        public static object Get(object primaryKey, Type type)
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

            Console.WriteLine(cmd.CommandText);

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

        public static Type[] GetSubtypes(Type type)
        {
            var tmpSubtypes = Assembly
                    .GetAssembly(type)
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(type));
            return tmpSubtypes.ToArray();
        }

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

        public static Dictionary<string, object> TransformReader(IDataReader reader)
        {
            return Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);
        }

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
                throw new OrmDuplicateException(nameof(Create), "Entry with specified id does already exist in database.", ex);
            }
            cmd.Dispose();

            SetReferences(table, obj);

            cache.CacheObject(obj);
        }

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
                throw new OrmNotFoundException(nameof(Create), "Entry with specified exception does not exist in database.", ex);
            }
            cmd.Dispose();

            SetReferences(table, obj);

            cache.CacheObject(obj);
        }

        public static void Remove(object obj)
        {
            Table table = GetTable(obj);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {table.Name} WHERE {table.PrimaryKey.Name} = :v1";
            
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ":v1";
            param.Value = table.PrimaryKey.GetObjectValue(obj);
            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            cache.RemoveObject(obj);
        }

        public static Table GetTable(object obj)
        {
            if (obj is Type)
                return new Table((Type)obj);
            return new Table(obj.GetType());
        }

        static object CreateObject(Type type, Dictionary<string,object> reader)
        {
            if(reader.ContainsKey("discriminator"))
            {
                var classType = reader["discriminator"];
                var asm = type.Assembly;
                type = asm.GetTypes().Single(t => t.Name == classType.ToString());
            }

            var table = GetTable(type);

            var pk = table.PrimaryKey.ToCodeType(reader[table.PrimaryKey.Name.ToLower()], cache);

            object value = null;
            
            if(cache.ContainsKey(pk))
            {
                value = cache.GetObject(pk);
            }
            else
            {
                value = cache.SearchTmp(pk);
            }

            if (value == null)
            {
                value = Activator.CreateInstance(type);
                cache.AddTmp(value);
            }
            else
                return value;

            foreach (var col in table.TableCols)
            {
                col.SetObjectValue(value, col.ToCodeType(reader[col.Name.ToLower()], cache));
            }

            foreach (var col in table.ReferencedCols)
            {
                col.SetObjectValue(value, col.FillReferencedColumns(Activator.CreateInstance(col.MemberType), value, type));
            }

            return value;
        }

        public static void IncludeReferencedColumns(Type type, object list, string sql, Dictionary<string, object> parameters)
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

        private static void SetReferences(Table table, object obj)
        {
            foreach(var col in table.ReferencedCols)
            {
                col.SetReferences(obj);
            }
        }

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
    }
}
