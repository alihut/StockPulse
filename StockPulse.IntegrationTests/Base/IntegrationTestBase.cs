using Microsoft.AspNetCore.SignalR.Client;
using StockPulse.IntegrationTests.Fixtures;
using StockPulse.IntegrationTests.Helpers;
using System.Net.Http.Headers;

namespace StockPulse.IntegrationTests.Base
{
    [Collection("IntegrationTestCollection")]
    public abstract class IntegrationTestBase
    {
        protected readonly IntegrationTestFixture Factory;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(IntegrationTestFixture fixture)
        {
            Factory = fixture;
            Client = Factory.CreateClient();
        }

        protected async Task<string> LoginAsAsync(string username = "user1", string password = "Password123")
        {
            return await AuthHelper.LoginAsync(Client, username, password);
        }

        protected void AuthenticateClient(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected HubConnection? CreateAlertHubConnectionAsync(string token)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"{Client.BaseAddress}alerts", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                    options.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler();
                })
                .WithAutomaticReconnect()
                .Build();
            return connection;
        }

    }

}
