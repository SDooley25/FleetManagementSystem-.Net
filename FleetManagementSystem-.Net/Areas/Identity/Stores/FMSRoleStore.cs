using FleetManagementSystem_.Net.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace FleetManagementSystem_.Net.Areas.Identity.Stores
{
    public class FMSRoleStore : IRoleStore<FMSRole>
    {
        public void Dispose()
        {
            // IDbConnection is controlled by DI container; do not dispose here unless you created it.
        }

        public async Task<IdentityResult> CreateAsync(FMSRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to insert role
            // stored proc: usp_CreateRole

            return await Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> UpdateAsync(FMSRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to update role by Id
            // stored proc: usp_UpdateRole

            return await Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> DeleteAsync(FMSRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to delete role by Id
            // stored proc: usp_DeleteRole

            return await Task.FromResult(IdentityResult.Success);
        }

        public async Task<FMSRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to find role by Id
            // stored proc: usp_FindRoleById

            return await Task.FromResult<FMSRole?>(null);
        }

        public async Task<FMSRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO: Call stored procedure to find role by normalized name
            // stored proc: usp_FindRoleByName

            return await Task.FromResult<FMSRole?>(null);
        }

        public Task<string?> GetRoleIdAsync(FMSRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult<string?>(role.Id);
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
