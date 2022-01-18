using Npgsql;
using SWE3_ORM_App.ModelClasses;
using SWE3_ORM_Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_App
{
    public static class Showcase
    {
        static Teacher teacher;

        static Class c1;
        static Class c2;

        static Student s1;
        static Student s2;

        static Course co1;
        static Course co2;

        public static void Setup()
        {
            string connectionString = ConfigurationManager.AppSettings["connectionString"];
            ORMapper.StartConnection(new NpgsqlConnection(connectionString));

            ORMapper.ResetDatabase();
        }

        public static void CreateTeacher()
        {
            Console.WriteLine("-- Creating Teacher --");

            teacher = new Teacher();
            teacher.Id = "t.1";
            teacher.Name = "Fritz";
            teacher.FirstName = "Ferdinand";
            teacher.BDate = new DateTime(1990, 8, 26);
            teacher.HDate = new DateTime(2020, 5, 4);
            teacher.Salary = 50000;
            teacher.Gender = Gender.Male;

            ORMapper.Create(teacher);

            Console.WriteLine("-- Teacher inserted into database --");
            Console.WriteLine();
        }

        public static void CreateClasses()
        {
            Console.WriteLine("-- Creating Classes --");

            c1 = new Class();
            c1.Id = "c.1";
            c1.Name = "A1";
            c1.kTeacher = teacher;

            c2 = new Class();
            c2.Id = "c.2";
            c2.Name = "A2";
            c2.kTeacher = teacher;

            ORMapper.Create(c1);
            ORMapper.Create(c2);



            Console.WriteLine("-- Classes inserted into database --");
            Console.WriteLine();
        }

        public static void CreateStudents()
        {
            Console.WriteLine("-- Creating Students --");

            s1 = new Student();
            s1.Id = "s.0";
            s1.Name = "Moped";
            s1.FirstName = "Manuel";
            s1.Gender = Gender.Male;
            s1.BDate = new DateTime(1999, 5, 5);
            s1.Grade = 1;
            s1.kClass = c1;

            s2 = new Student();
            s2.Id = "s.1";
            s2.Name = "Moped";
            s2.FirstName = "Manuela";
            s2.Gender = Gender.Female;
            s2.BDate = new DateTime(1999, 5, 5);
            s2.Grade = 3;
            s2.kClass = c1;

            ORMapper.Create(s1);
            ORMapper.Create(s2);
            c1.Students.Add(s1);
            c1.Students.Add(s2);
            ORMapper.Update(c1);

            Console.WriteLine("-- Students inserted into database --");
            Console.WriteLine();
        }

        public static void CreateCourses()
        {
            Console.WriteLine("-- Creating Courses --");

            co1 = new Course();
            co1.Id = "co.0";
            co1.HActive = 1;
            co1.Name = "Latin";
            co1.kTeacher = teacher;

            co2 = new Course();
            co2.Id = "co.1";
            co2.HActive = 1;
            co2.Name = "French";
            co2.kTeacher = teacher;

            ORMapper.Create(co1);
            ORMapper.Create(co2);

            Console.WriteLine("-- Courses inserted into database --");
            Console.WriteLine();
        }

        public static void AddStudentsToCourses()
        {
            Console.WriteLine("-- Adding Students to courses --");

            co1.students.Add(s1);
            co1.students.Add(s2);
            ORMapper.Update(co1);

            co2.students.Add(s1);
            ORMapper.Update(co2);

            Console.WriteLine("-- Courses updated with students --");
            Console.WriteLine();
        }

        public static void SelectTeacherWithClasses()
        {
            Console.WriteLine("-- Selecting Teacher from database by primary key --");

            ORMapper.ClearCache();

            var t = (Teacher)ORMapper.GetByPK(teacher.Id, typeof(Teacher));

            Console.WriteLine($"{t.Id} {t.FirstName} {t.Name} {t.BDate} {t.Gender} {t.HDate} {t.Salary}");
            foreach (var s in t.Classes)
                Console.WriteLine($"\t{s.Id} {s.Name}");

            Console.WriteLine("-- Adding Students to courses --");
            Console.WriteLine();
        }
    }
}
