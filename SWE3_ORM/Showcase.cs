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
        }

        public static void CreateTables()
        {
            Console.WriteLine("-- Creating Tables for relevant classes --");

            ORMapper.CreateTable(typeof(Person));
            ORMapper.CreateTable(typeof(Class));
            ORMapper.CreateTable(typeof(Course));

            Console.WriteLine("-- Adding relationships to those tables in the form of foreign keys and middle tables for MtoN relationships --");

            ORMapper.AddRelationshipConstraints(new Type[] { typeof(Person), typeof(Class), typeof(Course) });

            Console.WriteLine("-- Tables completely created --");
            Console.WriteLine();
        }

        public static void Reset()
        {
            Console.WriteLine("-- Resetting database --");
            ORMapper.ResetDatabase();
            Console.WriteLine();
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
            s1.Id = "s.1";
            s1.Name = "Moped";
            s1.FirstName = "Manuel";
            s1.Gender = Gender.Male;
            s1.BDate = new DateTime(1999, 5, 5);
            s1.Grade = 1;
            s1.kClass = c1;

            s2 = new Student();
            s2.Id = "s.2";
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
            co1.Id = "co.1";
            co1.HActive = 1;
            co1.Name = "Latin";
            co1.kTeacher = teacher;

            co2 = new Course();
            co2.Id = "co.2";
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

            Console.WriteLine($"Primary key is {teacher.Id}");
            var t = (Teacher)ORMapper.GetByPK(teacher.Id, typeof(Teacher));

            Console.WriteLine($"{t.Id} {t.FirstName} {t.Name} {t.BDate} {t.Gender} {t.HDate} {t.Salary}");
            Console.WriteLine("Classes:");
            foreach (var s in t.Classes)
                Console.WriteLine($"\t{s.Id} {s.Name}");

            Console.WriteLine("-- Data selected --");
            Console.WriteLine();
        }

        public static void SelectClassWithStudents()
        {
            Console.WriteLine("-- Selecting Classes from database by primary key --");

            Console.WriteLine($"Primary key is {c1.Id}");
            var c = (Class)ORMapper.GetByPK(c1.Id, typeof(Class));

            Console.WriteLine($"{c.Id} {c.Name} Teacher: {c.kTeacher.Name}");
            Console.WriteLine("Students:");
            foreach (var s in c.Students)
                Console.WriteLine($"\t{s.Id} {s.Name} {s.FirstName} {s.Gender}");

            Console.WriteLine("-- Data selected --");
            Console.WriteLine();
        }

        public static void SelectCoursesAndStudents()
        {
            Console.WriteLine("-- Selecting Courses and Students from database by primary key --");

            Console.WriteLine($"Primary key is {co1.Id}");
            var c = (Course)ORMapper.GetByPK(co1.Id, typeof(Course));

            Console.WriteLine($"{c.Id} {c.Name} Teacher: {c.kTeacher.Name} {c.kTeacher.FirstName}");
            Console.WriteLine("Students:");
            foreach (var s in c.students)
                Console.WriteLine($"\t{s.Id} {s.Name} {s.FirstName} {s.Gender} {s.Grade}");

            Console.WriteLine();

            Console.WriteLine($"Primary key is {co2.Id}");
            c = (Course)ORMapper.GetByPK(co2.Id, typeof(Course));

            Console.WriteLine($"{c.Id} {c.Name} Teacher: {c.kTeacher.Name} {c.kTeacher.FirstName}");
            Console.WriteLine("Students:");
            foreach (var s in c.students)
                Console.WriteLine($"\t{s.Id} {s.Name} {s.FirstName} {s.Gender} {s.Grade}");

            Console.WriteLine("-- Data selected --");
            Console.WriteLine();
        }

        public static void SelectStudentsByParamters()
        {
            Console.WriteLine("-- Selecting Students by Parameters --");

            Console.WriteLine($"Parameters: ('Name' = 'Moped')");
            var s = ORMapper.GetByParams<Student>(new List<Tuple<string, object>>() { new Tuple<string, object>("name", "Moped") });

            Console.WriteLine("Students:");
            foreach (var student in s)
                Console.WriteLine($"{student.Id} {student.Name} {student.FirstName} {student.Gender} {student.Grade} {student.BDate}");
            Console.WriteLine();

            Console.WriteLine($"Parameters: ('Id' = 's.1')");
            s = ORMapper.GetByParams<Student>(new List<Tuple<string, object>>() { new Tuple<string, object>("id", "s.1") });
            Console.WriteLine("Students:");
            foreach (var student in s)
                Console.WriteLine($"{student.Id} {student.Name} {student.FirstName} {student.Gender} {student.Grade} {student.BDate}");

            Console.WriteLine("-- Data selected --");
            Console.WriteLine();
        }

        public static void SelectTeacherBySql()
        {
            Console.WriteLine("-- Selecting Teacher by sql --");
            var sql = "SELECT * FROM persons WHERE id = :v0";

            Console.WriteLine($"Sql: '{sql}' with :v0 = 't.1'");
            var t = ORMapper.GetBySql<Teacher>(sql, new Dictionary<string, object>() { { ":v0", "t.1" } });

            Console.WriteLine($"{t[0].Id} {t[0].FirstName} {t[0].Name} {t[0].BDate} {t[0].Gender} {t[0].HDate} {t[0].Salary}");

            Console.WriteLine("-- Data selected --");
            Console.WriteLine();
        }

        public static void RemoveCourse()
        {
            Console.WriteLine("-- Removing course with id = 'co.2' --");

            Console.WriteLine("Before Removing:");
            var co = ORMapper.GetBySql<Course>("SELECT * FROM courses", new Dictionary<string, object>());

            foreach (var c in co)
                Console.WriteLine(c.Id);
            Console.WriteLine();

            Console.WriteLine($"Removing ...");
            ORMapper.Remove(co2);
            Console.WriteLine();

            Console.WriteLine("After Removing:");
            co = ORMapper.GetBySql<Course>("SELECT * FROM courses", new Dictionary<string, object>());

            foreach (var c in co)
                Console.WriteLine(c.Id);

            Console.WriteLine("-- Course removed --");
            Console.WriteLine();
        }
    }
}
