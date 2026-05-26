using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FleetManagementSystem_.Net.Data
{
    public interface IVehicleRepository
    {
        Task<Guid> InsertAsync(Vehicle vehicle, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Vehicle vehicle, CancellationToken cancellationToken);
        Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken);
    }

    public class VehicleRepository : IVehicleRepository
    {
        private readonly ILogger<VehicleRepository> _logger;
        private readonly string _connectionString;

        public VehicleRepository(IConfiguration configuration, ILogger<VehicleRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString();
        }

        public async Task<Guid> InsertAsync(Vehicle vehicle, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Insert - Vehicle : {Make} {Model}", vehicle.Make, vehicle.Model);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("iVehicle", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", vehicle.Id);
            command.AddParameterWithValue("Make", vehicle.Make);
            command.AddParameterWithValue("Model", vehicle.Model);
            command.AddParameterWithValue("FuelType", vehicle.FuelType);
            command.AddParameterWithValue("EngineSize", vehicle.EngineSize);
            command.AddParameterWithValue("DateOfRegistration", vehicle.DateOfRegistration);
            command.AddParameterWithValue("LastMotDate", vehicle.LastMotDate);
            command.AddParameterWithValue("LastMotMileage", vehicle.LastMotMileage);
            command.PrepareCommand();

            var result = await command.ExecuteScalarAsync(cancellationToken);
            if (result is Guid newKey && newKey != Guid.Empty)
            {
                vehicle.Id = newKey;
                return vehicle.Id;
            }

            return Guid.Empty;
        }

        public async Task<bool> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Update - VehicleId : {Id}", vehicle.Id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("uVehicle", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", vehicle.Id);
            command.AddParameterWithValue("Make", vehicle.Make);
            command.AddParameterWithValue("Model", vehicle.Model);
            command.AddParameterWithValue("FuelType", vehicle.FuelType);
            command.AddParameterWithValue("EngineSize", vehicle.EngineSize);
            command.AddParameterWithValue("DateOfRegistration", vehicle.DateOfRegistration);
            command.AddParameterWithValue("LastMotDate", vehicle.LastMotDate);
            command.AddParameterWithValue("LastMotMileage", vehicle.LastMotMileage);
            command.PrepareCommand();

            var rows = await command.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Vehicle vehicle, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Delete - VehicleId : {Id}", vehicle.Id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("dVehicle", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", vehicle.Id);
            command.PrepareCommand();

            var rows = await command.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0;
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetById - VehicleId : {Id}", id);
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("sVehicle", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", id);
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (reader.HasRows)
            {
                return new Vehicle(reader);
            }
            return null;
        }

        public async Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetAll Vehicles");
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("slistVehicle", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var list = Vehicle.GetListColumns(reader);
            return list;
        }
    }
}
