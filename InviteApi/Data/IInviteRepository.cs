using InviteApiContract.Commands;
using System;
using System.Threading.Tasks;

namespace InviteApi.Data
{
    public interface IInviteRepository
    {
        Task<int> GetInviteCount(int apiId, DateTime curDate);
        Task<string[]> GetUsedPhones(int apiId, string[] phones);
        Task SaveInvite(int apiId, InviteCommand invite);
    }
}