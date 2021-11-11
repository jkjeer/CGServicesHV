using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace CGServicesHV.Common.Types.Response
{
    public static class GenericListExtensions
    {
        public static GenericListDataReader<T> GetDataReader<T>(this IEnumerable<T> list)
        {
            return new GenericListDataReader<T>(list);
        }

        public static DataTable GetDataTable<T>(IList<T> list)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }

    public class GenericListDataReader<T> : IDataReader
    {
        private IEnumerator<T> list = null;
        private List<PropertyInfo> properties = new List<PropertyInfo>();
        private Dictionary<string, int> nameLookup = new Dictionary<string, int>();

        public GenericListDataReader(IEnumerable<T> list)
        {
            this.list = list.GetEnumerator();

            properties.AddRange(
                typeof(T)
                .GetProperties(
                    BindingFlags.GetProperty |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.DeclaredOnly
                    ));

            for (int i = 0; i < properties.Count; i++)
            {
                nameLookup[properties[i].Name] = i;
            }
        }

        #region IDataReader Members

        public void Close()
        {
            list.Dispose();
        }

        public int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public DataTable GetSchemaTable()
        {
            var dt = new DataTable("SchemaTable");
            dt.Columns.Add(new DataColumn(SchemaTableColumn.ColumnName, typeof(string)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(int)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.ColumnSize, typeof(int)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.NumericPrecision, typeof(short)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.NumericScale, typeof(short)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.DataType, typeof(Type)));
            dt.Columns.Add(new DataColumn(SchemaTableOptionalColumn.ProviderSpecificDataType, typeof(Type)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.NonVersionedProviderType, typeof(int)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.ProviderType, typeof(int)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.IsLong, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.AllowDBNull, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.IsUnique, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.IsKey, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsHidden, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(string)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(string)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.BaseTableName, typeof(string)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.BaseColumnName, typeof(string)));
            dt.Columns.Add(new DataColumn(SchemaTableOptionalColumn.BaseServerName, typeof(string)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.IsAliased, typeof(bool)));
            dt.Columns.Add(new DataColumn(SchemaTableColumn.IsExpression, typeof(bool)));

            for (int i = 0; i < this.FieldCount; i++)
            {
                DataRow row = dt.NewRow();
                row["ColumnName"] = this.GetName(i);
                row["ColumnOrdinal"] = i;
                row["ColumnSize"] = -1;
                Type type = this.GetFieldType(i);
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<int>).GetGenericTypeDefinition())
                {
                    type = type.GetGenericArguments()[0];
                }
                row["DataType"] = this.GetFieldType(i);
                dt.Rows.Add(row);
            }
            return dt;
        }

        public bool IsClosed
        {
            get { throw new NotImplementedException(); }
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            return list.MoveNext();
        }

        public int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get { return properties.Count; }
        }

        public bool GetBoolean(int i)
        {
            return (bool)GetValue(i);
        }

        public byte GetByte(int i)
        {
            return (byte)GetValue(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)GetValue(i);
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
            return properties[i].PropertyType.Name;
            //throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)GetValue(i);
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)GetValue(i);
        }

        public double GetDouble(int i)
        {
            return (double)GetValue(i);
        }

        public Type GetFieldType(int i)
        {
            return properties[i].PropertyType;
        }

        public float GetFloat(int i)
        {
            return (float)GetValue(i);
        }

        public Guid GetGuid(int i)
        {
            return (Guid)GetValue(i);
        }

        public short GetInt16(int i)
        {
            return (short)GetValue(i);
        }

        public int GetInt32(int i)
        {
            return (int)GetValue(i);
        }

        public long GetInt64(int i)
        {
            return (long)GetValue(i);
        }

        public string GetName(int i)
        {
            return properties[i].Name;
        }

        public int GetOrdinal(string name)
        {
            if (nameLookup.ContainsKey(name))
            {
                return nameLookup[name];
            }
            else
            {
                return -1;
            }
        }

        public string GetString(int i)
        {
            return (string)GetValue(i);
        }

        public object GetValue(int i)
        {
            return properties[i].GetValue(list.Current, null);
        }

        public int GetValues(object[] values)
        {
            int getValues = Math.Max(FieldCount, values.Length);

            for (int i = 0; i < getValues; i++)
            {
                values[i] = GetValue(i);
            }

            return getValues;
        }

        public bool IsDBNull(int i)
        {
            return GetValue(i) == null;
        }

        public object this[string name]
        {
            get
            {
                return GetValue(GetOrdinal(name));
            }
        }

        public object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        #endregion
    }
}
