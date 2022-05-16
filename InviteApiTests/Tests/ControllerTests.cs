using InviteApi.Controllers;
using InviteApi.Data;
using InviteApi.Services;
using InviteApi.Settings;
using InviteApiTests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace InviteApiTests.Tests
{
    public class ControllerTests
    {
        [Fact]
        public async Task SendInviteWhenLimitIsExceeded_Return403()
        {
            var apiSettings = Options.Create<ApiSettings>(new ApiSettings
            {
                Id = 4,
                Limit = 128
            });
            var invite = new InviteCommandBuilder()
                .Message("Test message")
                .Phones(3)
                .Build();
            var smsServiceMoq = new Mock<ISmsSender>();
            var repository = new Mock<IInviteRepository>();
            repository
                .Setup(r => r.GetInviteCount(apiSettings.Value.Id, It.IsAny<DateTime>()))
                .Returns(Task.FromResult(129));
            var controller = new InviteController(apiSettings, smsServiceMoq.Object, repository.Object);

            var result = await controller.SendAsync(invite);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, objResult.StatusCode);
        }
    }
}
