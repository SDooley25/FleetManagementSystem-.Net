using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FleetManagementSystem_.Net.Areas.Identity.Stores
{
    public class CompromisedPasswordRepository
    {
        private readonly string _connString;
        private readonly ILogger<CompromisedPasswordRepository> _logger;

        public CompromisedPasswordRepository(IConfiguration configuration, ILogger<CompromisedPasswordRepository> logger)
        {
            _connString = configuration.GetConnectionString();
            _logger = logger;
        }

        public async Task<Guid> CreateAsync(CompromisedPassword item, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Insert compromised hash");
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand("iCompromisedPassword", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", item.Id);
            command.AddParameterWithValue("PasswordHash", item.PasswordHash);
            command.AddParameterWithValue("DateAdded", item.DateAdded);
            command.PrepareCommand();

            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is Guid g ? g : item.Id;
        }

        public async Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Delete compromised hash {Id}", id);
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand("dCompromisedPassword", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", id);
            command.PrepareCommand();

            var rows = await command.ExecuteScalarAsync(cancellationToken);
            return rows is int i ? i : 0;
        }

        public async Task<List<CompromisedPassword>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Get all compromised hashes");
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand("slistCompromisedPassword", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.PrepareCommand();
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var list = CompromisedPassword.GetListColumns(reader);
            return list;
        }

        public async Task<List<CompromisedPassword>> FindByHashPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Find compromised hashes by prefix");
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand("sCompromisedPasswordByHash", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("PasswordHashPrefix", prefix);
            command.PrepareCommand();

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var list = CompromisedPassword.GetListColumns(reader);
            return list;
        }
    }
}
