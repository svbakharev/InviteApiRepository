using InviteApi;
using InviteApiTests.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;
using InviteApi.Data;

namespace InviteApiTests.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _fixture;

        public IntegrationTests(WebApplicationFactory<Startup> fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SendTwoIdenticalInvites_FirstReturnOk_SecondReturnBadRequest()
        {
            HttpClient client = GetClient();
            var invite = new InviteCommandBuilder()
                .Message("Test message")
                .Phones(3)
                .Build();            
            var content = new StringContent(
                JsonSerializer.Serialize(invite),
                Encoding.UTF8,
                Application.Json);

            using var response = await client.PostAsync("/api/invite/send", content);
            response.EnsureSuccessStatusCode();

            using var response2 = await client.PostAsync("/api/invite/send", content);
            Assert.Equal(StatusCodes.Status400BadRequest, (int)response2.StatusCode);
        }

        [Fact]
        public async Task SendInviteWithEmptyMessage_ReturnCode405()
        {
            HttpClient client = GetClient();
            var invite = new InviteCommandBuilder()
                .Message("")
                .Phones(3)
                .Build();
            var content = new StringContent(
                JsonSerializer.Serialize(invite),
                Encoding.UTF8,
                Application.Json);

            using var response = await client.PostAsync("/api/invite/send", content);
            Assert.Equal(405, (int)response.StatusCode);
            Assert.Equal("Invite message is missing", await response.Content.ReadAsStringAsync());
        }

        private HttpClient GetClient()
        {
            return _fixture.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices
                            .GetRequiredService<AppDbContext>();

                        db.Invites.RemoveRange(db.Invites);
                        db.SaveChanges();
                    }

                });
            }).CreateClient();
        }
    }
}
