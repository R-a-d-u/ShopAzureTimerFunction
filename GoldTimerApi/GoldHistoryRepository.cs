using GoldFetchTimer.HelperClass;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using System.Data;
using System.Text.Json;

public class GoldHistoryRepository : IGoldHistoryRepository
{
    private readonly string _connectionString;
    private readonly GoldApiFetch _apiFetch;
    private readonly ILogger<GoldHistoryRepository> _logger;

    public GoldHistoryRepository(string connectionString, GoldApiFetch apiFetch, ILogger<GoldHistoryRepository> logger)
    {
        _connectionString = connectionString;
        _apiFetch = apiFetch;
        this._logger = logger;
    }

    public async Task<bool> AddGoldHistoryAsync()
    {
        // First check if there's already a record for today
        if (await HasTodayRecordAsync())
        {
            throw new InvalidOperationException("A gold history record already exists for today's date.");
        }

        // Fetch gold price data from API
        var goldPriceDataJson = await _apiFetch.GetGoldPriceFilteredAsync();

        // Check if we got an error message back
        if (goldPriceDataJson.StartsWith("Request error") || goldPriceDataJson.StartsWith("Unexpected error"))
        {
            throw new InvalidOperationException($"Unable to fetch gold price data: {goldPriceDataJson}");
        }

        // Deserialize the JSON response
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TimestampConverter());
        var goldPriceData = JsonSerializer.Deserialize<GoldPriceData>(goldPriceDataJson, options);

        if (goldPriceData == null)
        {
            throw new InvalidOperationException("Failed to parse gold price data.");
        }

        // Insert the gold history record
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            try
            {
                // Start a transaction to ensure both the insert and the stored procedure call succeed
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await InsertGoldHistoryAsync(connection, goldPriceData, transaction);

                        // Call the stored procedure to update product prices
                        await UpdateProductPricesBasedOnGoldPriceAsync(connection, transaction);

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback if any error occurs
                        transaction.Rollback();
                        _logger.LogError(ex, "Failed to add gold history record or update product prices.");
                        throw;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add gold history record.");
                throw;
            }
        }
    }

    private async Task<bool> HasTodayRecordAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand("SELECT TOP 1 1 FROM shop.GoldHistory WHERE CONVERT(date, Date) = CONVERT(date, GETDATE())", connection))
            {
                var result = await command.ExecuteScalarAsync();
                return result != null;
            }
        }
    }

    private async Task<int> InsertGoldHistoryAsync(SqlConnection connection, GoldPriceData goldData, SqlTransaction transaction)
    {
        string sql = @"
            INSERT INTO shop.GoldHistory (Metal, PriceOunce, PriceGram, PercentageChange, Exchange, Timestamp, Date)
            VALUES (@Metal, @PriceOunce, @PriceGram, @PercentageChange, @Exchange, @Timestamp, @Date);
            SELECT SCOPE_IDENTITY();";

        using (var command = new SqlCommand(sql, connection, transaction))
        {
            command.Parameters.AddWithValue("@Metal", goldData.Metal);
            command.Parameters.AddWithValue("@PriceOunce", goldData.PriceOunce);
            command.Parameters.AddWithValue("@PriceGram", goldData.PriceGram);
            command.Parameters.AddWithValue("@PercentageChange", goldData.PercentageChange);
            command.Parameters.AddWithValue("@Exchange", goldData.Exchange);
            command.Parameters.AddWithValue("@Timestamp", goldData.Timestamp);
            command.Parameters.AddWithValue("@Date", DateTime.Today);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }

    private async Task UpdateProductPricesBasedOnGoldPriceAsync(SqlConnection connection, SqlTransaction transaction)
    {
        // Call the stored procedure dbo.UpdateProductPricesBasedOnGoldPrice
        using (var command = new SqlCommand("dbo.UpdateProductPricesBasedOnGoldPrice", connection, transaction))
        {
            command.CommandType = CommandType.StoredProcedure;

            // If your stored procedure requires parameters, add them here
            // command.Parameters.AddWithValue("@YourParameter", yourValue);

            await command.ExecuteNonQueryAsync();
        }
    }
}

public class GoldPriceData
    {
        public string Metal { get; set; }
        public decimal PriceOunce { get; set; }
        public decimal PriceGram { get; set; }
        public decimal PercentageChange { get; set; }
        public string Exchange { get; set; }
        public string Timestamp { get; set; }
    }

    public interface IGoldHistoryRepository
    {
        Task<bool> AddGoldHistoryAsync();
    }
