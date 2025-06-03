using System.Net.Http.Json;
using System.Text.Json;

namespace StockPulse.IntegrationTests.Helpers
{
    public static class AuthHelper
    {
        public static async Task<string> LoginAsync(HttpClient client, string username, string password)
        {
            var response = await client.PostAsJsonAsync("/api/auth/login", new
            {
                username,
                password
            });

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            // Assumes your API returns: { "token": "..." }
            return json.GetProperty("token").GetString()!;
        }
    }

}
