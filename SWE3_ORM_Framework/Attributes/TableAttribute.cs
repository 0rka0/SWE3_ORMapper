using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.Attributes
{
    /// <summary>
    /// The table attribute is used to map classes onto database tables.
    /// </summary>
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// The name of the table.
        /// Default name will be the name of the class.
        /// </summary>
        public string Name = null;

        /// <summary>
        /// Used to enable a discriminator that will be added to the table if it is true.
        /// Necessary for Inheritance. Base and derived classes will be saved in a single table.
        /// Discriminators are used to differenciate between different classes that are saved in the same database table.
        /// </summary>
        public bool Discriminator = false;
    }
}
