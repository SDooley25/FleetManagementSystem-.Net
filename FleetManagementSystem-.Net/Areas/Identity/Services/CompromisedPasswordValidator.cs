using FleetManagementSystem_.Net.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FleetManagementSystem_.Net.Areas.Identity.Services
{
    public class CompromisedPasswordValidator : IPasswordValidator<FMSUser>
    {
        private readonly ICompromisedPasswordService _service;
        private readonly ILogger<CompromisedPasswordValidator> _logger;

        public CompromisedPasswordValidator(ICompromisedPasswordService service, ILogger<CompromisedPasswordValidator> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<FMSUser> manager, FMSUser user, string password)
        {
            try
            {
                var compromised = await _service.IsCompromisedAsync(password);
                if (compromised)
                {
                    _logger.LogWarning("Password for user {User} is compromised", user?.UserName);
                    var error = new IdentityError
                    {
                        Code = "CompromisedPassword",
                        Description = "This password appears in a list of compromised passwords. Choose a different password."
                    };
                    return IdentityResult.Failed(error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating compromised password");
                // In case of error, do not block registration; return success to avoid false positives.
            }

            return IdentityResult.Success;
        }
    }
}
