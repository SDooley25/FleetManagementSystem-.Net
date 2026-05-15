using Microsoft.Data.SqlClient;
using System.Data;

namespace FleetManagementSystem_.Net.Extensions
{
    public static class SqlCommandExtensions
{
    public static void AddParameterWithValue(this SqlCommand command, string name, object? value, ParameterDirection direction = ParameterDirection.Input, bool addToEndOfCommandText = false)
    {
        if (string.IsNullOrWhiteSpace(command.CommandText))
        {
            throw new Exception("start the command");
        }

        if (value == null)
        {
            value = DBNull.Value;
        }

        if (!name.StartsWith("@"))
        {
            name = "@" + name;
        }

        SqlParameter value2 = new SqlParameter
        {
            ParameterName = name,
            Value = value,
            Direction = direction
        };
        command.Parameters.Add(value2);
        string commandText = command.CommandText;
        int length = commandText.Length;
        int num = length - 1;
        if (commandText.Substring(num, length - num) != " ")
        {
            command.CommandText += " ";
        }

        if (addToEndOfCommandText)
        {
            command.CommandText = command.CommandText + name + ",";
        }
    }

    public static void PrepareCommand(this SqlCommand command)
    {
        string commandText = command.CommandText;
        int length = commandText.Length;
        int num = length - 1;
        if (commandText.Substring(num, length - num) == ",")
        {
            string commandText2 = command.CommandText;
            command.CommandText = commandText2.Substring(0, commandText2.Length - 1);
        }
    }

    public static void AddParameterListWithValue(this SqlCommand command, Dictionary<string, object> parameters)
    {
        foreach (KeyValuePair<string, object> parameter in parameters)
        {
            command.AddParameterWithValue(parameter.Key, parameter.Value);
        }
    }
}
}
