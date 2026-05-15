
using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FleetManagementSystem_.Net.Areas.Identity.Stores
{
    public class FMSUserStore 
        : IUserStore<FMSUser>,
        IUserPasswordStore<FMSUser>,
        IUserRoleStore<FMSUser>,
        IUserLockoutStore<FMSUser>,
        IUserEmailStore<FMSUser>,
        IDisposable
    {

        private readonly string _connString;
        private readonly ILogger<FMSUserStore> _logger;

        public FMSUserStore(IConfiguration configuration,
            ILogger<FMSUserStore> logger)
        {
            _connString = configuration.GetConnectionString();                
            _logger = logger;
        }        

        // ---- IUserStore ----
        public async Task<IdentityResult> CreateAsync(FMSUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Insert - Username : {Username}", user.UserName);
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync();
    
            using var command = new SqlCommand("iUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.AddParameterWithValue("Id", user.Id);
            command.AddParameterWithValue("UserName", user.UserName);
            command.AddParameterWithValue("NormalizedUserName", user.NormalizedUserName);
            command.AddParameterWithValue("Email", user.Email);
            command.AddParameterWithValue("NormalizedEmail", user.NormalizedEmail);
            command.AddParameterWithValue("EmailConfirmed", user.EmailConfirmed);
            command.AddParameterWithValue("PasswordHash", user.PasswordHash);
            command.PrepareCommand();
            var result = await command.ExecuteScalarAsync();
    
            var newKey = (Guid)result;
            if (newKey != Guid.Empty)
            {
                user.Id = newKey;
                return IdentityResult.Success;
            }
            return IdentityResult.Failed();            
        }

        public async Task<IdentityResult> UpdateAsync(FMSUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Insert - Username : {Username}", user.UserName);
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync();
    
            using var command = new SqlCommand("uUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.AddParameterWithValue("UserName", user.UserName);
            command.AddParameterWithValue("NormalizedUserName", user.NormalizedUserName);
            command.AddParameterWithValue("Email", user.Email);
            command.AddParameterWithValue("NormalizedEmail", user.NormalizedEmail);
            command.AddParameterWithValue("EmailConfirmed", user.EmailConfirmed);
            command.AddParameterWithValue("PasswordHash", user.PasswordHash);
            command.AddParameterWithValue("LockoutEnabled", user.LockoutEnabled);
            command.AddParameterWithValue("LockoutEnd", user.LockoutEnd);
            command.AddParameterWithValue("AccessFailedCount", user.AccessFailedCount);
            command.PrepareCommand();
            var result = await command.ExecuteScalarAsync();
    
            var newKey = (Guid)result;
            if (newKey != Guid.Empty)
            {
                user.Id = newKey;
                return IdentityResult.Success;
            }
            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> DeleteAsync(FMSUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("FindById - userId : {UserId}", user.Id);
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync();
    
            using var command = new SqlCommand("sUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.AddParameterWithValue("Id", user.Id);
            command.PrepareCommand();

            var rowsAffected = await command.ExecuteNonQueryAsync();
    
            if (rowsAffected > 0)
            {
                return IdentityResult.Success;
            }
            return IdentityResult.Failed();
        }

        public async Task<FMSUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("FindById - userId : {UserId}", userId);
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync();
    
            using var command = new SqlCommand("sUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.AddParameterWithValue("Id", userId);
            command.PrepareCommand();

            using var reader = await command.ExecuteReaderAsync();
    
            if (reader.HasRows)
            {
                return new FMSUser(reader);
            }
            return null;
        }

        public async Task<FMSUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("FindByName - normalizedUserName : {NormalizedUserName}", normalizedUserName);
            using var connection = new SqlConnection(_connString);
            await connection.OpenAsync();
    
            using var command = new SqlCommand("sUserByNormName", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.AddParameterWithValue("NormalizedUserName", normalizedUserName);
            command.PrepareCommand();

            using var reader = await command.ExecuteReaderAsync();
    
            if (reader.HasRows)
            {
                return new FMSUser(reader);
            }
            return null;
        }

        public Task<string?> GetUserIdAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string?>(user.Id.ToString());
        }

        public Task<string?> GetUserNameAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(FMSUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string?> GetNormalizedUserNameAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(FMSUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        // ---- IUserPasswordStore ----
        public Task SetPasswordHashAsync(FMSUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string?> GetPasswordHashAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        // ---- IUserRoleStore ----
        public async Task AddToRoleAsync(FMSUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to add mapping user->role.
            // stored proc: usp_AddUserToRole (parameters: @UserId, @RoleName or @RoleId)
        }

        public async Task RemoveFromRoleAsync(FMSUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to remove mapping user->role.
            // stored proc: usp_RemoveUserFromRole
        }

        public async Task<IList<string>> GetRolesAsync(FMSUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to return role names for the user.
            // stored proc: usp_GetRolesForUser
            return await Task.FromResult((IList<string>)new List<string>());
        }

        public async Task<bool> IsInRoleAsync(FMSUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to check membership
            // stored proc: usp_IsUserInRole
            return await Task.FromResult(false);
        }

        public async Task<IList<FMSUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to return users in a role
            // stored proc: usp_GetUsersInRole
            return await Task.FromResult((IList<FMSUser>)new List<FMSUser>());
        }

        // ---- IUserLockoutStore ----
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(FMSUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(FMSUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount++;
            // Optionally persist increment via stored proc: usp_IncrementAccessFailedCount
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(FMSUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            // Optionally persist reset via stored proc: usp_ResetAccessFailedCount
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(FMSUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //nothing to dispose
        }

        public Task SetEmailAsync(FMSUser user, string? email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string?> GetEmailAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(FMSUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<FMSUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetNormalizedEmailAsync(FMSUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(FMSUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }
    }
}
