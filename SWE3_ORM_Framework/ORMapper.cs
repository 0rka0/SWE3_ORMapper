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

        //private static Dictionary<Type, Table> tables = new Dictionary<Type, Table>();

        public static void StartConnection(IDbConnection connection)
        {
            Connection = connection;
            Connection.Open();
        }

        public static object Get(object primaryKey, Type type)
        {
            object value = null;
            Table table = GetTable(type);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {table.Name} WHERE ";

            var tmpSubtypes = Assembly
               .GetAssembly(type)
               .GetTypes()
               .Where(t => t.IsSubclassOf(type));
            var subtypes = tmpSubtypes.ToArray();

            if (table.Discriminator != null)
            {
                IDataParameter dParam = cmd.CreateParameter();
                cmd.CommandText += $"({nameof(table.Discriminator)} = :v2";
                dParam.ParameterName = ":v2";
                dParam.Value = table.Discriminator;
                cmd.Parameters.Add(dParam);

                for (int i = 0; i < subtypes.Length; i++)
                {
                    IDataParameter ddParam = cmd.CreateParameter();
                    var pName = ":v" + (i + 3);
                    cmd.CommandText += $" OR {nameof(table.Discriminator)} = {pName}";
                    ddParam.ParameterName = pName;
                    ddParam.Value = subtypes[i].Name;
                    cmd.Parameters.Add(ddParam);
                }
                cmd.CommandText += ") AND ";
            }

            IDataParameter pkParam = cmd.CreateParameter();
            cmd.CommandText += $"{table.PrimaryKey.Name} = :v1";
            pkParam.ParameterName = ":v1";
            pkParam.Value = primaryKey;
            cmd.Parameters.Add(pkParam);

            Console.WriteLine(cmd.CommandText);

            IDataReader reader = cmd.ExecuteReader();
            List<object> values = new List<object>();
            if (reader.Read())
            {
                value = CreateObject(type, reader);
            }

            reader.Close();
            cmd.Dispose();

            return value;
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
        }

        public static void Update(object obj)
        {
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
        }

        public static Table GetTable(object obj)
        {
            if (obj is Type)
                return new Table((Type)obj);
            return new Table(obj.GetType());
        }

        static object CreateObject(Type type, IDataReader reader)
        {
            try
            {
                var classType = reader.GetValue(reader.GetOrdinal("discriminator"));

                if (classType != DBNull.Value)
                {
                    var asm = type.Assembly;
                    type = asm.GetTypes().Single(t => t.Name == classType.ToString());
                }
            }
            catch { }

            var table = GetTable(type);
            object value = Activator.CreateInstance(type);

            foreach (var col in table.TableCols)
            {
                col.SetObjectValue(value, col.ToCodeType(reader.GetValue(reader.GetOrdinal(col.Name))));
            }

            foreach (var col in table.ReferencedCols)
            {
                col.SetObjectValue(value, col.FillReferencedColumns(Activator.CreateInstance(col.MemberType), value));
            }

            return value;
        }

        public static void IncludeReferencedColumns(Type type, object list, string sql, IEnumerable<Tuple<string, object>> parameters)
        {

        }
    }
}
