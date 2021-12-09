using System;

namespace SWE3_ORM_Framework.Attributes
{
    public class ColumnAttribute : Attribute
    {
        public string Name = null;

        public Type Type = null;

        public bool Nullable = false;
    }
}
