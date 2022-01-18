using System;
using System.Configuration;
using System.Collections.Specialized;
using SWE3_ORM_Framework;
using Npgsql;
using SWE3_ORM_App.ModelClasses;
using SWE3_ORM_App.ShowCase;
using SWE3_ORM_App;

namespace SWE3_ORM
{
    class Program
    {
        static void Main(string[] args)
        {
            Showcase.Setup();
            Showcase.CreateTeacher();
            Showcase.CreateClasses();
            Showcase.CreateStudents();
            Showcase.CreateCourses();
            Showcase.AddStudentsToCourses();
            Showcase.SelectTeacherWithClasses();
        }
    }
}
