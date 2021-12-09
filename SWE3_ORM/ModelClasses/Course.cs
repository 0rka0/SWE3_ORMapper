using SWE3_ORM_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_App.ModelClasses
{
    [Table(Name = "courses")]
    public class Course
    {
        [PrimaryKey]
        public string Id { get; set; }

        public int HActive { get; set; }

        public string Name { get; set; }

        [ForeignKey]
        public Teacher kTeacher { get; set; }

        [ForeignKey(TargetTable = "student_courses", TargetColumn = "kstudent")]
        public List<Student> kCourse { get; set; } = new List<Student>();
    }
}
