using AngularNetBase.Practice.Dtos.Rewards;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AngularNetBase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RewardsController : ControllerBase
    {
        private readonly IRewardService _rewardService;

        public RewardsController(IRewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [HttpGet("current")]
        public async Task<ActionResult<RewardProgressDto?>> GetCurrent(CancellationToken cancellationToken)
        {
            var result = await _rewardService.GetCurrentRewardAsync(GetUserId(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("gallery")]
        public async Task<ActionResult<RewardGalleryDto>> GetGallery(CancellationToken cancellationToken)
        {
            var result = await _rewardService.GetGalleryAsync(GetUserId(), cancellationToken);
            return Ok(result);
        }

        [HttpPost("{rewardProgressId:guid}/save")]
        public async Task<ActionResult<RewardProgressDto>> Save(
            Guid rewardProgressId,
            CancellationToken cancellationToken)
        {
            var result = await _rewardService.SaveRewardAsync(GetUserId(), rewardProgressId, cancellationToken);
            return Ok(result);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
