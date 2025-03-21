using FinBeat_Tech_Test.Models;
using Dapper;
using Npgsql;

namespace FinBeat_Tech_Test.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ILogger<DatabaseService> _logger;
        private readonly string? _connectionString;
        
        public DatabaseService(ILogger<DatabaseService> logger, IConfiguration configuration) 
        {
            _logger = logger;
            _connectionString = configuration.GetSection("ConnectionString")?.GetValue<string>("ValueTable");
            if (String.IsNullOrEmpty(_connectionString))
            {
                _logger.LogError("ConnectionString in configuration is empty. Set default connection");
                _connectionString = "Host=localhost;Username=postgres;Password=password;Database=ValueTable";
            }
            DatabaseAvailabilityCheck().Wait(); //Ожидание, пока выполнится создание таблицы (вдруг это займет много времени)
        }

        private async Task DatabaseAvailabilityCheck()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS values (
                    id SERIAL PRIMARY KEY,
                    code INT NOT NULL,
                    value TEXT NOT NULL
                );";

                await connection.ExecuteAsync(createTableQuery);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Database error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task ClearValuesAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.ExecuteAsync("DELETE FROM values");

                _logger.LogInformation("Table values is clear");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<Dictionary<int, Values>> GetAllValuesAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var result = await connection.QueryAsync<Values>("SELECT * FROM values");
                return result.ToDictionary(x => x.Id, t => t);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task SaveValuesAsync(Dictionary<int, Values> values)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                
                await ClearValuesAsync();

                foreach (var value in values.Values)
                {
                    await connection.ExecuteAsync("INSERT INTO values (code, value) VALUES (@Code, @Value)", value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
