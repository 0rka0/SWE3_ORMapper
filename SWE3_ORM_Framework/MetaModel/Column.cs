﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Framework.MetaModel
{
    public class Column
    {
        public Table Table { get; private set; }

        public PropertyInfo Member { get; set; }

        public Type MemberType 
        { 
            get
            {
                return Member.PropertyType;
            }    
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public bool IsPK { get; set; } = false;

        public bool IsFK { get; set; } = false;

        public string TargetTable { get; set; }

        public string TargetColumn { get; set; }

        public bool Nullable { get; set; } = true;

        public bool IsReferencedCol { get; set; } = false;

        public bool IsMtoN { get; set; }

        public Column(Table table)
        {
            Table = table;
        }

        public object GetObjectValue(object obj)
        {
            var value = Member.GetValue(obj);

            return value;
        }

        public void SetObjectValue(object obj, object value)
        {
            Member.SetValue(obj, value);
            return;
        }

        public object ToColumnType(object obj)
        {
            if (IsFK)
            {
                if (obj == null)
                    return null;

                return ORMapper.GetTable(MemberType).PrimaryKey.ToColumnType(ORMapper.GetTable(MemberType).PrimaryKey.GetObjectValue(obj));
            }

            if (obj is Enum)
            {
                if (Type == typeof(short))
                {
                    return (short)obj;
                }
                if (Type == typeof(long))
                {
                    return (long)obj;
                }
                return (int)obj;
            }

            if (MemberType == Type)
                return obj;

            if (obj is bool)
            {
                if(Type == typeof(int))
                {
                    if ((bool)obj)
                        return 1;
                    return 0;
                }
                if (Type == typeof(short))
                {
                    if ((bool)obj)
                        return (short)1;
                    return (short)0;
                }
                if (Type == typeof(long))
                {
                    if ((bool)obj)
                        return (long)1;
                    return (long)0;
                }
            }

            return obj;
        }

        public object ToCodeType(object obj)
        {
            if (IsFK)
            {
                return ORMapper.Get(obj, MemberType);
            }

            if (MemberType == typeof(bool))
            {
                if (obj is int)
                    return (int)obj != 0;
                if (obj is short)
                    return (short)obj != 0;
                if (obj is long)
                    return (long)obj != 0;
            }

            if (MemberType == typeof(short))
                return (short)obj;
            if (MemberType == typeof(int))
                return (int)obj;
            if (MemberType == typeof(long))
                return (long)obj;

            if (Type.IsEnum)
                return Enum.ToObject(Type, (int)obj);

            return obj;
        }

        public object FillReferencedColumns(object list, object obj)
        {
            ORMapper.IncludeReferencedColumns(MemberType.GenericTypeArguments[0], list, GetReferenceSql(), 
                new Tuple<string, object>[] { new Tuple<string, object>(":fk", Table.PrimaryKey.GetObjectValue(obj)) } );
        
            return list;
        }

        string GetReferenceSql()
        {
            string value;
            Table table = ORMapper.GetTable(MemberType.GenericTypeArguments[0]);
            if (IsMtoN)
            {
                
            }
            return "";
        }
    }
}
