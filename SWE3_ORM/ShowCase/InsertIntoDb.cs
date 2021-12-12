using SWE3_ORM_App.ModelClasses;
using SWE3_ORM_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_App.ShowCase
{
    public static class InsertIntoDb
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

            try
            {
                //ORMapper.Create(teacher);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var test = ORMapper.Get("t.0", typeof(Person));
        }
    }
}
