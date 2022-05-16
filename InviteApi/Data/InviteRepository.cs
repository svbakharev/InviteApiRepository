using InviteApi.Data;
using InviteApiContract.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InviteApi.Data
{
    public class InviteRepository : IInviteRepository
    {
        private readonly AppDbContext _context;

        public InviteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetInviteCount(int apiId, DateTime curDate)
        {
            return await _context.Invites
                .Where(i => i.ApiId == apiId)
                .Where(i => i.CreateDate.Date == curDate.Date)
                .Select(i => i.Id)
                .CountAsync();
        }

        public async Task<string[]> GetUsedPhones(int apiId, string[] phones) 
        {
            return await _context.Invites.Include(i => i.Phones)
                .Where(i => i.ApiId == apiId)
                .SelectMany(i => i.Phones)
                .Where(i => phones.Contains(i.Phone))
                .Select(i => i.Phone)
                .ToArrayAsync();
        }

        public async Task SaveInvite(int apiId, InviteCommand inviteCommand)
        {
            var invite = new Invite
            {
                ApiId = apiId,
                CreateDate = DateTime.Now,
                Message = inviteCommand.Message,
                Phones = inviteCommand.Phones.Select(p => new InvitePhone { Phone = p }).ToList()
            };

            _context.Invites.Add(invite);
            await _context.SaveChangesAsync();
        }
    }
}
