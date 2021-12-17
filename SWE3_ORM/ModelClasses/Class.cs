using SWE3_ORM_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_App.ModelClasses
{
    [Table(Name = "classes")]
    public class Class
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Name { get; set; }

        [ForeignKey]
        public Teacher kTeacher { get; set; }

        [ForeignKey(Name = "kclass")]
        public List<Student> Students { get; set; } = new List<Student>();
    }
}
