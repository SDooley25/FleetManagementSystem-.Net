using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Extensions;
using FleetManagementSystem_.Net.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Data;

namespace FleetManagementSystem_.Net.Data
{
    public interface IStorageSiteRepository
    {
        Task<Guid> InsertAsync(StorageSite site, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(StorageSite site, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(StorageSite site, CancellationToken cancellationToken);
        Task<StorageSite?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<StorageSite>> GetAllAsync(CancellationToken cancellationToken);
    }
    public class StorageSiteRepository : IStorageSiteRepository
    {
        private ILogger<StorageSiteRepository> _logger;
        private string _connectionString;

        public StorageSiteRepository(IConfiguration configuration, ILogger<StorageSiteRepository> logger)
        {
            _logger= logger;
            _connectionString= configuration.GetConnectionString();
        }

        public async Task<Guid> InsertAsync(StorageSite site, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Insert - StorageSite : {Name}", site.Name);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("iStorageSite", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            command.AddParameterWithValue("Id", site.Id);
            command.AddParameterWithValue("Name", site.Name);
            command.AddParameterWithValue("Address", site.Address);
            command.AddParameterWithValue("Postcode", site.Postcode);
            command.AddParameterWithValue("MaxVehicleCapacity", site.MaxVehicleCapacity);
            command.AddParameterWithValue("HasRepairStation", site.HasRepairStation);
            command.AddParameterWithValue("InUseDate", site.InUseDate);
            command.AddParameterWithValue("OutUseDate", site.OutUseDate);
            command.PrepareCommand();

            var result = await command.ExecuteScalarAsync(cancellationToken);

            if (result is Guid newKey && newKey != Guid.Empty)
            {
                site.Id = newKey;
                return site.Id;
            }

            return Guid.Empty;
        }

        public async Task<bool> UpdateAsync(StorageSite site, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Update - StorageSiteId : {Id}", site.Id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("uStorageSite", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", site.Id);
            command.AddParameterWithValue("Name", site.Name);
            command.AddParameterWithValue("Address", site.Address);
            command.AddParameterWithValue("Postcode", site.Postcode);
            command.AddParameterWithValue("MaxVehicleCapacity", site.MaxVehicleCapacity);
            command.AddParameterWithValue("HasRepairStation", site.HasRepairStation);
            command.AddParameterWithValue("InUseDate", site.InUseDate);
            command.AddParameterWithValue("OutUseDate", site.OutUseDate);
            command.PrepareCommand();

            var result = await command.ExecuteNonQueryAsync(cancellationToken);

            return result > 0;//result is num of rows affected, if more than 0 then success.
            
        }

        public async Task<bool> DeleteAsync(StorageSite site, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Delete - StorageSiteId : {Id}", site.Id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("dStorageSite", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", site.Id);
            command.PrepareCommand();

            var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

            return rowsAffected > 0;
        }

        public async Task<StorageSite?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("GetById - StorageSiteId : {Id}", id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("sStorageSite", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", id);
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows) { 
                var site = new StorageSite(reader);
                return site;
            }

            return null;
        }

        public async Task<List<StorageSite>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetAll StorageSites");
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("slistStorageSite", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var list = StorageSite.GetListColumns(reader);
            return list;
        }
    }
}
