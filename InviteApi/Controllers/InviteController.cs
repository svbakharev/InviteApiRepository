using System;
using System.Threading.Tasks;
using InviteApi.Data;
using InviteApi.Fiters;
using InviteApi.Services;
using InviteApi.Settings;
using InviteApiContract.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InviteApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class InviteController : Controller
    {
        private readonly ApiSettings _apiSettings;
        private readonly ISmsSender _smsSender;
        private readonly IInviteRepository _inviteRepository;

        public InviteController(IOptions<ApiSettings> apiSettings, ISmsSender smsSender, IInviteRepository inviteRepository)
        {
            _apiSettings = apiSettings.Value;
            _smsSender = smsSender;
            _inviteRepository = inviteRepository;
        }
        
        [HttpPost("send")]
        [InviteCommandFilter]
        public async Task<IActionResult> SendAsync([FromBody] InviteCommand invite)
        {
            try
            {
                if (await _inviteRepository.GetInviteCount(_apiSettings.Id, DateTime.Now) >= _apiSettings.Limit)
                {
                    return StatusCode(403, "Too much phone numbers, should be less or equal to 128 per day");
                }

                var usedPhones = await _inviteRepository.GetUsedPhones(_apiSettings.Id, invite.Phones);
                if (usedPhones.Length > 0)
                {
                    return BadRequest($"Phones: {String.Join(", ", usedPhones)} already have an invitation");
                }

                await _smsSender.Send(invite.Message, invite.Phones);
                await _inviteRepository.SaveInvite(_apiSettings.Id, invite);

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            
            return Ok();
        }
    }
}