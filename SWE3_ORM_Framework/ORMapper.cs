using SWE3_ORM_Framework.MetaModel;
using System;
using System.Collections.Generic;
using System.Data;

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

        public static void Create(object obj)
        {
            Table table = CreateTable(obj);
        }

        static Table CreateTable(object obj)
        {
            if (obj is Type)
                return new Table((Type)obj);
            return new Table(obj.GetType());
        }
    }
}
