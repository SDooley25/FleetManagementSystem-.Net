using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FleetManagementSystem_.Net.Data
{
    public sealed record VehicleStorageCapacityCheck(int VehicleCount, int MaxVehicleCapacity, DateTime CheckDate);

    public interface IVehicleStorageRepository
    {
        Task<Guid> InsertAsync(VehicleStorage item, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(VehicleStorage item, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(VehicleStorage item, CancellationToken cancellationToken);
        Task<VehicleStorage?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<VehicleStorage>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<VehicleStorage>> GetByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken);
        Task<List<VehicleStorage>> GetBySiteAsync(Guid siteId, CancellationToken cancellationToken);
        Task<VehicleStorageCapacityCheck?> GetVehicleCountBySiteAndDateAsync(Guid siteId, DateTime checkDate, CancellationToken cancellationToken);
        Task<bool> HasCapacityAvailableAsync(VehicleStorage item, CancellationToken cancellationToken);

    }

    public class VehicleStorageRepository : IVehicleStorageRepository
    {
        private readonly ILogger<VehicleStorageRepository> _logger;
        private readonly string _connectionString;

        public VehicleStorageRepository(IConfiguration configuration, ILogger<VehicleStorageRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString();
        }

        public async Task<Guid> InsertAsync(VehicleStorage item, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Insert - VehicleStorage : {Id}", item.Id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("iVehicleStorage", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", item.Id);
            command.AddParameterWithValue("VehicleId", item.Vehicle.Id);
            command.AddParameterWithValue("StorageSiteId", item.StorageSite.Id);
            command.AddParameterWithValue("StorageType", item.StorageType);
            command.AddParameterWithValue("StartDate", item.StartDate);
            command.AddParameterWithValue("EndDate", item.EndDate);
            command.AddParameterWithValue("Note", item.Note);
            command.PrepareCommand();

            var result = await command.ExecuteScalarAsync(cancellationToken);
            if (result is Guid newKey && newKey != Guid.Empty)
            {
                item.Id = newKey;
                return item.Id;
            }
            return Guid.Empty;
        }

        public async Task<bool> UpdateAsync(VehicleStorage item, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Update - VehicleStorageId : {Id}", item.Id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("uVehicleStorage", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", item.Id);
            command.AddParameterWithValue("VehicleId", item.Vehicle.Id);
            command.AddParameterWithValue("StorageSiteId", item.StorageSite.Id);
            command.AddParameterWithValue("StorageType", item.StorageType);
            command.AddParameterWithValue("StartDate", item.StartDate);
            command.AddParameterWithValue("EndDate", item.EndDate);
            command.AddParameterWithValue("Note", item.Note);
            command.PrepareCommand();

            var rows = await command.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(VehicleStorage item, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Delete - VehicleStorageId : {Id}", item.Id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("dVehicleStorage", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", item.Id);
            command.PrepareCommand();

            var rows = await command.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0;
        }

        public async Task<VehicleStorage?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetById - VehicleStorageId : {Id}", id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("sVehicleStorageFull", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", id);
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (reader.HasRows)
            {
                return new VehicleStorage(reader);
            }
            return null;
        }

        public async Task<List<VehicleStorage>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetAll VehicleStorage");
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("slistVehicleStorageFull", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var list = VehicleStorage.GetListColumns(reader);
            return list;
        }

        public async Task<List<VehicleStorage>> GetByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetByVehicle - VehicleId: {VehicleId}", vehicleId);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("slistVehicleStorageByVehicleFull", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.AddParameterWithValue("VehicleId", vehicleId);
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return VehicleStorage.GetListColumns(reader);
        }

        public async Task<List<VehicleStorage>> GetBySiteAsync(Guid siteId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetBySite - SiteId: {SiteId}", siteId);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("slistVehicleStorageBySiteFull", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.AddParameterWithValue("StorageSiteId", siteId);
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return VehicleStorage.GetListColumns(reader);
        }

        public virtual async Task<VehicleStorageCapacityCheck?> GetVehicleCountBySiteAndDateAsync(Guid siteId, DateTime checkDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetVehicleCountBySiteAndDate - SiteId: {SiteId} Date: {CheckDate}", siteId, checkDate.Date);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("sVehicleCountBySiteAndDate", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("SiteID", siteId);
            command.AddParameterWithValue("CheckDate", checkDate.Date);
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!reader.HasRows || !await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            var vehicleCount = reader.Get<int>("VehicleCount");
            var maxVehicleCapacity = reader.Get<int>("MaxVehicleCapacity");
            var returnedDate = reader.Get<DateTime>("CheckDate");

            return new VehicleStorageCapacityCheck(vehicleCount, maxVehicleCapacity, returnedDate);
        }

        public virtual async Task<bool> HasCapacityAvailableAsync(VehicleStorage item, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var startDate = item.StartDate.Date;
            var endDate = item.EndDate?.Date ?? startDate;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var result = await GetVehicleCountBySiteAndDateAsync(item.StorageSite.Id, date, cancellationToken);
                if (result == null || result.VehicleCount >= result.MaxVehicleCapacity)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
