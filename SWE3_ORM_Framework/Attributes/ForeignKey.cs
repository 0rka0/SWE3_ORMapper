using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.Attributes
{
    /// <summary>
    /// Extends the column attribute and marks a property as a foreign key in the database.
    /// </summary>
    public class ForeignKey : ColumnAttribute
    {
        /// <summary>
        /// Defines the middle table when dealing with MtoN relationships.
        /// </summary>
        public string TargetTable = null;

        /// <summary>
        /// Defines the column for the references of the related objects.
        /// </summary>
        public string TargetColumn = null;
    }
}
