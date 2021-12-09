using SWE3_ORM_Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_App.ModelClasses
{
    [Table(Name = "persons", Discriminator = "person")]
    public class Person
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public Gender Gender { get; set; }

        public DateTime BDate { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
