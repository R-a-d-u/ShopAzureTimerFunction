using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GoldFetchTimer.HelperClass
{
    public class GoldApiFetch
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoldApiFetch(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["ApiSettings:GoldApiKey"]
                      ?? throw new InvalidOperationException("Gold API key is missing in User Secrets or appsettings.json.");
        }

        public async Task<string> GetGoldPriceFilteredAsync()
        {
            string url = "https://www.goldapi.io/api/XAU/USD";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("x-access-token", _apiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                JsonElement root = doc.RootElement;

                var filteredData = new
                {
                    Metal = root.GetProperty("metal").GetString(),
                    PriceOunce = root.GetProperty("price").GetDecimal(),
                    PriceGram = root.GetProperty("price_gram_24k").GetDecimal(),
                    PercentageChange = root.GetProperty("chp").GetDecimal(),
                    Timestamp = root.GetProperty("timestamp").GetInt64(),
                    Exchange = root.GetProperty("exchange").GetString()
                };

                return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (HttpRequestException ex)
            {
                return $"Request error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected error: {ex.Message}";
            }
        }
    }
}
