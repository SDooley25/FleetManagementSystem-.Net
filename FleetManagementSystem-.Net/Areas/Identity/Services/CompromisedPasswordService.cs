using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Stores;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace FleetManagementSystem_.Net.Areas.Identity.Services
{
    public interface ICompromisedPasswordService
    {
        Task<bool> IsCompromisedAsync(string password, CancellationToken cancellationToken = default);
    }

    public class CompromisedPasswordService : ICompromisedPasswordService
    {
        private readonly CompromisedPasswordRepository _repo;
        private readonly ILogger<CompromisedPasswordService> _logger;

        public CompromisedPasswordService(CompromisedPasswordRepository repo, ILogger<CompromisedPasswordService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<bool> IsCompromisedAsync(string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            var sha1 = ComputeSha1Hex(password).ToUpperInvariant();
            // Use k-anonymity style: query by small prefix to limit DB work
            var prefix = sha1.Length > 5 ? sha1.Substring(0, 5) : sha1;

            try
            {
                var matches = await _repo.FindByHashPrefixAsync(prefix, cancellationToken);
                if (matches != null && matches.Count > 0)
                {
                    return matches.Any(m => string.Equals(m.PasswordHash, sha1, StringComparison.OrdinalIgnoreCase));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking compromised password list");
            }

            return false;
        }

        private static string ComputeSha1Hex(string input)
        {
            using var sha1 = SHA1.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha1.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
