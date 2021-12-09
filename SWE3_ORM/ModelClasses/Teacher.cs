using SWE3_ORM_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_App.ModelClasses
{
    [Table(Name = "persons", Discriminator = "teacher")]
    public class Teacher : Person
    {
        public DateTime HDate { get; set; }

        public int Salary { get; set; }

        public List<Class> Classes { get; set; } = new List<Class>();
    }
}
