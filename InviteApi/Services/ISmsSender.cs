using System.Threading.Tasks;

namespace InviteApi.Services
{
    public interface ISmsSender
    {
        public Task Send(string message, string[] phones);
    }
}