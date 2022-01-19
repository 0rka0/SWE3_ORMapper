using SWE3_ORM_App;

namespace SWE3_ORM
{
    class Program
    {
        static void Main(string[] args)
        {
            Showcase.Setup();
            Showcase.CreateTables();
            Showcase.Reset();
            Showcase.CreateTeacher();
            Showcase.CreateClasses();
            Showcase.CreateStudents();
            Showcase.CreateCourses();
            Showcase.AddStudentsToCourses();
            Showcase.SelectTeacherWithClasses();
            Showcase.SelectClassWithStudents();
            Showcase.SelectCoursesAndStudents();
            Showcase.SelectStudentsByParamters();
            Showcase.SelectTeacherBySql();
            Showcase.RemoveCourse();
        }
    }
}
