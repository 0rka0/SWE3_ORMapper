using System;
using System.Configuration;
using System.Collections.Specialized;
using SWE3_ORM_Framework;
using Npgsql;
using SWE3_ORM_App.ModelClasses;
using SWE3_ORM_App.ShowCase;

namespace SWE3_ORM
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"];
            ORMapper.StartConnection(new NpgsqlConnection(connectionString));

            Test.Run();
        }
    }
}
