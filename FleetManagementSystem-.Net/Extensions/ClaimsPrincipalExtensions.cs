using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace FleetManagementSystem_.Net.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        private const string DefaultSuperAdminRole = "SuperAdmin";

        public static bool IsInRoleLike(this ClaimsPrincipal user, string rolePattern)
        {
            return user.IsInRoleLike(rolePattern, DefaultSuperAdminRole);
        }

        public static bool IsInRoleLike(this ClaimsPrincipal user, string rolePattern, IConfiguration configuration)
        {
            var superAdminRole = configuration?["Auth:SuperAdminRole"] ?? DefaultSuperAdminRole;
            return user.IsInRoleLike(rolePattern, superAdminRole);
        }

        public static bool IsInRoleLike(this ClaimsPrincipal user, string rolePattern, string superAdminRole)
        {
            if (user == null || string.IsNullOrWhiteSpace(rolePattern))
            {
                return false;
            }

            // Super admin always allowed
            if (!string.IsNullOrWhiteSpace(superAdminRole) && user.IsInRole(superAdminRole))
            {
                return true;
            }

            // Exact role check if no wildcard
            if (!rolePattern.EndsWith(".*", StringComparison.Ordinal))
            {
                return user.IsInRole(rolePattern);
            }

            // Handle area wildcard like "AreaName.*"
            var areaRolePrefix = rolePattern.Substring(0, rolePattern.Length - 1);
            if (string.IsNullOrWhiteSpace(areaRolePrefix))
            {
                return false;
            }

            //Get all role names from user claims
            var roleClaims = user.Claims
                .Where(claim => string.Equals(claim.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase)
                                || string.Equals(claim.Type, "role", StringComparison.OrdinalIgnoreCase))
                .Select(claim => claim.Value)
                .Where(value => !string.IsNullOrWhiteSpace(value));

            //check if any role claim starts with the area prefix
            foreach (var claimValue in roleClaims)
            {
                if (claimValue.StartsWith(areaRolePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
