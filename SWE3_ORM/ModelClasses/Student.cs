using SWE3_ORM_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_App.ModelClasses
{
    public class Student : Person
    {
        public int Grade { get; set; }

        [ForeignKey]
        public Class kClass { get; set; }

        [ForeignKey(TargetTable = "student_courses", Name = "kstudent", TargetColumn = "kcourse")]
        public List<Course> courses { get; set; } = new List<Course>();
    }
}
