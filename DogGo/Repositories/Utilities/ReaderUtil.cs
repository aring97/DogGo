using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories.Utilities
{
    public class ReaderUtil
    {
        public static string GetNullableString(SqlDataReader reader, string ColumnName)
        {
            if (!reader.IsDBNull(reader.GetOrdinal(ColumnName))){
                return reader.GetString(reader.GetOrdinal(ColumnName));
            }
            else
            {
                return null;
            }
        }
        public static object GetNullParam(object value)
        {
            if (value != null)
            {
                return value;
            }
            else
            {
                return DBNull.Value;
            }
        }
    }
}
