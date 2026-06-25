using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace FleetManagementSystem_.Net.Areas.Identity.Stores
{
    public class FMSRoleStore : IRoleStore<FMSRole>, IQueryableRoleStore<FMSRole>
    {
        private readonly string _connString;
        private readonly ILogger<FMSRoleStore> _logger;

        private IQueryable<FMSRole> _roles;

        public IQueryable<FMSRole> Roles
        {
            get 
            {
                if (_roles == null)
                {
                    _roles = GetAllRolesAsync(CancellationToken.None).GetAwaiter().GetResult();
                }
                return _roles;
            }
        }

        public FMSRoleStore(IConfiguration configuration,
            ILogger<FMSRoleStore> logger)
        {
            _connString = configuration.GetConnectionString();
            _logger = logger;
        }

        public void Dispose()
        {
            // IDbConnection is controlled by DI container; do not dispose here unless you created it.
        }

        public async Task<IdentityResult> CreateAsync(FMSRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Insert - RoleName : {RoleName}", role.Name);
            await using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("iRole", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            command.AddParameterWithValue("Id", role.Id);
            command.AddParameterWithValue("Name", role.Name);
            command.AddParameterWithValue("NormalizedName", role.NormalizedName);
            command.AddParameterWithValue("Description", role.Description);
            command.PrepareCommand();

            var result = await command.ExecuteScalarAsync(cancellationToken);

            if (result is Guid newKey && newKey != Guid.Empty)
            {
                role.Id = newKey;
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> UpdateAsync(FMSRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Update - RoleId : {RoleId}", role.Id);
            await using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("uRole", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            
            command.AddParameterWithValue("Id", role.Id);
            command.AddParameterWithValue("Name", role.Name);
            command.AddParameterWithValue("NormalizedName", role.NormalizedName);
            command.AddParameterWithValue("Description", role.Description);
            command.PrepareCommand();

            var result = await command.ExecuteScalarAsync(cancellationToken);

            if (result is Guid updatedKey && updatedKey != Guid.Empty)
            {
                role.Id = updatedKey;
                return IdentityResult.Success;
            }

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> DeleteAsync(FMSRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Delete - RoleId : {RoleId}", role.Id);
            await using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("dRole", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("Id", role.Id);
            command.PrepareCommand();

            var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

            if (rowsAffected > 0)
            {
                return IdentityResult.Success;
            }
            return IdentityResult.Failed();
        }

        public async Task<FMSRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(roleId, out var roleGuid))
            {
                throw new ArgumentException("Invalid role ID format.", nameof(roleId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("FindById - roleId : {RoleId}", roleId);
            await using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("sRole", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            command.AddParameterWithValue("Id", roleGuid);
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows)
            {
                var role = new FMSRole(reader);
                return role;
            }

            return null;
        }

        public async Task<FMSRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("FindByName - normalizedRoleName : {NormalizedRoleName}", normalizedRoleName);
            await using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand("sRoleByNormName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.AddParameterWithValue("NormalizedName", normalizedRoleName);
            command.PrepareCommand();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows)
            {
                var role = new FMSRole(reader);               
                return role;
            }

            return null;
        }

        public async Task<IQueryable<FMSRole>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation("GetAllRoles");
            await using var connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new SqlCommand("slistRole", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.PrepareCommand();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var roles = FMSRole.GetListColumns(reader);
            return roles.AsQueryable();
        }

        public Task<string?> GetRoleIdAsync(FMSRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string?>(role.Id.ToString());
        }

        public Task<string?> GetRoleNameAsync(FMSRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(FMSRole role, string? roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedRoleNameAsync(FMSRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(FMSRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }
    }
}
