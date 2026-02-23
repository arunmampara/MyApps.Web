using Api.Auth.Models;

namespace Api.Auth.Services
{
    public class LoginService : ILoginService
    {
        private readonly IConfiguration _configuration;

        public LoginService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Login(Models.Account userAccount)
        {
            if (!string.IsNullOrEmpty(userAccount.UserName) && !string.IsNullOrEmpty(userAccount.Password))
            {
                // Simulate an asynchronous operation
                string? connectionString = _configuration.GetConnectionString("ConnectionString");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'ConnectionString' is not configured.");
                }

                using var db = new DatabaseManager(connectionString);
                var rowsAffected = await db.ExecuteScalarAsync(
                    "ValidateUser",
                    new Dictionary<string, object>
                    {
                        { "@UserName", userAccount.UserName },
                        { "@Password", userAccount.Password }
                    }
                );

                return true;
            }

            return false;
        }

        public async Task<bool> Register(Account userAccount)
        {
            if (!string.IsNullOrEmpty(userAccount.UserName) && !string.IsNullOrEmpty(userAccount.Password))
            {
                // Simulate an asynchronous operation
                string? connectionString = _configuration.GetConnectionString("ConnectionString");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'ConnectionString' is not configured.");
                }

                using var db = new DatabaseManager(connectionString);
                var rowsAffected = await db.ExecuteNonQueryAsync(
                    "dbo.CreateClient",
                    new Dictionary<string, object>
                    {
                        { "@UserName", userAccount.UserName },
                        { "@Password", userAccount.Password }
                    }
                );
                return true;
            }
            return false;
        }
    }
}