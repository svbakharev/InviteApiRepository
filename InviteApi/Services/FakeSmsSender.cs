using System.Threading.Tasks;

namespace InviteApi.Services
{
    public class FakeSmsSender : ISmsSender
    {
        public Task Send(string message, string[] phones)
        {
            return Task.CompletedTask;
        }
    }
}
