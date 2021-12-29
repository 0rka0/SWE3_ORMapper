using SWE3_ORM_App.ModelClasses;
using SWE3_ORM_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_App.ShowCase
{
    public static class Test
    {
        public static void Run()
        {
            var teacher = new Teacher();
            teacher.Id = "t.0";
            teacher.Name = "Fritz";
            teacher.FirstName = "Ferdinand";
            teacher.BDate = new DateTime(1990, 8, 26);
            teacher.HDate = new DateTime(2020, 5, 4);
            teacher.Salary = 50000;
            teacher.Gender = Gender.Male;
            teacher.Classes = new System.Collections.Generic.List<Class>() { new Class(), new Class() };

            var c = new Class();
            c.Id = "c.1";
            c.Name = "A2";
            c.kTeacher = teacher;

            var s1 = new Student();
            s1.Id = "s.0";
            s1.Name = "Moped";
            s1.FirstName = "Manuel";
            s1.Gender = Gender.Male;
            s1.BDate = new DateTime(1999, 5, 5);
            s1.Grade = 1;
            s1.kClass = c;

            var s2 = new Student();
            s2.Id = "s.1";
            s2.Name = "Moped";
            s2.FirstName = "Manuela";
            s2.Gender = Gender.Female;
            s2.BDate = new DateTime(1999, 5, 5);
            s2.Grade = 3;
            s2.kClass = c;

            var course1 = new Course();
            course1.Id = "co.0";
            course1.HActive = 1;
            course1.Name = "Latin";
            course1.kTeacher = teacher;

            var course2 = new Course();
            course2.Id = "co.1";
            course2.HActive = 1;
            course2.Name = "French";
            course2.kTeacher = teacher;

            try
            {
                //ORMapper.Create(course1);
                course1.students.Add(s1);
                course1.students.Add(s2);
                ORMapper.Update(course1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var test = ORMapper.Get("t.0", typeof(Teacher));
            Console.WriteLine(((Teacher)test).Id);
            Console.WriteLine(((Teacher)test).Classes[0].Id);
        }
    }
}
