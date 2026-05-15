using Microsoft.Data.SqlClient;
using System.Data;

namespace FleetManagementSystem_.Net.Extensions
{
    public static class DataReaderExtensions
    {
        public static T Get<T>(this IDataReader reader, int column)
        {
            if (reader.IsDBNull(column))
            {
                return default;
            }

            return (T)reader[column];
        }

        public static T Get<T>(this IDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
            {
                return default(T);
            }

            return (T)reader[ordinal];
        }

        public static T? GetNullableValue<T>(this SqlDataReader reader, int ordinal) where T : class
        {
            return reader.IsDBNull(ordinal) ? null : (reader.GetValue(ordinal) as T);
        }

        public static T? GetNullableValue<T>(this IDataReader reader, string columnName, T? nullReturnValue = null) where T : class
        {
            int ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
            {
                return nullReturnValue;
            }

            return (T)reader[ordinal];
        }
    }
}
