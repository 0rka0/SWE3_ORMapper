using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE3_ORM_Test
{
    public class MockDataReader : IDataReader
    {
        List<Dictionary<string, object>> entries;
        List<Dictionary<string, object>> foundEntries;
        Dictionary<string, object> curDict;

        List<string> curKeys;
        List<object> curValues;

        public int FieldCount = 1;

        public string searchId = "";

        public void Fill()
        {
            entries = new List<Dictionary<string, object>>();
            entries.Add(new Dictionary<string, object>()
            {
                { "id", "t.0" },
                { "discriminator", "Teacher" },
                { "name", "Fritz" },
                { "firstname", "Ferdinand" },
                { "gender", 0 },
                { "bdate", new DateTime(1990, 08, 26, 0, 0, 0) },
                { "hdate", new DateTime(2020, 05, 04, 0, 0, 0) },
                { "salary", 50000 },
                { "kclass", null },
                { "grade", null }
            });
            entries.Add(new Dictionary<string, object>()
            {
                { "id", "t.1" },
                { "discriminator", "Teacher" },
                { "name", "Fritz" },
                { "firstname", "Ferdinand" },
                { "gender", 0 },
                { "bdate", new DateTime(1990, 08, 26, 0, 0, 0) },
                { "hdate", new DateTime(2020, 05, 04, 0, 0, 0) },
                { "salary", 50000 },
                { "kclass", null },
                { "grade", null }
            });

            FieldCount = entries.First().Count;
        }

        public string GetName
        {
            get
            {
                var rval = curKeys.First();
                curKeys.Remove(rval);
                return rval;
            }
        }

        public object GetValue
        {
            get
            {
                var rval = curValues.First();
                curValues.Remove(rval);
                return rval;
            }
        }

        public bool Read()
        {
            foundEntries = entries.FindAll(d => d.ContainsValue(searchId));

            if (foundEntries.Count == 0)
                return false;

            curDict = foundEntries.First();

            entries.Remove(curDict);
            foundEntries.Remove(curDict);
            
            FieldCount = curDict.Count;

            curKeys = curDict.Keys.ToList();
            curValues = curDict.Values.ToList();

            return true;
        }

        string IDataRecord.GetName(int i)
        {
            return curKeys[i];
        }

        object IDataRecord.GetValue(int i)
        {
            return curValues[i];
        }

        public void Close()
        {           
        }

        public void Dispose()
        {
        }

        public int Depth => throw new NotImplementedException();

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => throw new NotImplementedException();

        int IDataRecord.FieldCount => FieldCount;

        public object this[string name] => throw new NotImplementedException();

        public object this[int i] => throw new NotImplementedException();

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }
    }
}
