using System;

namespace SWE3_ORM_Framework.Attributes
{
    /// <summary>
    /// The column attribute is used to map properties onto database columns.
    /// </summary>
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// The name of the column.
        /// Default name will be the name of the property.
        /// </summary>
        public string Name = null;

        /// <summary>
        /// The type of the column.
        /// Default type will be the type of the property.
        /// </summary>
        public Type Type = null;

        /// <summary>
        /// Defines if a column is nullable in the database.
        /// Default will be true.
        /// </summary>
        public bool Nullable = true;
    }
}
