using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.Attributes
{
    public class TableAttribute : Attribute
    {
        public string Name;

        public string Discriminator;

        //public string Child;
    }
}
