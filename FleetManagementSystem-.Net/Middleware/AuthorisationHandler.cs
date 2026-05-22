using Microsoft.AspNetCore.Authorization;

namespace FleetManagementSystem_.Net.Middleware
{
    public class AuthorisationHandler : IAuthorizationHandler
    {
        private string _superAdminRole;

        public AuthorisationHandler(IConfiguration config)
        {
           _superAdminRole = config["Auth:SuperAdminRole"] ?? "SuperAdmin";
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true &&
                context.User.IsInRole(_superAdminRole))
            {
                //find all role-based requirements and succeed them for SuperAdmin users
                //saves adding superadmin on to everywhere that deals with roles
                foreach (var requirement in context.Requirements.ToList())
                {
                    if(requirement?.ToString()?.ToLower().Contains("user.isinrole") == true)
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
