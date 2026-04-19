using AngularNetBase.Practice.Dtos.GrowthMessages;
using AngularNetBase.Practice.Entities.GrowthMessages;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AngularNetBase.API.Controllers
{
    [ApiController]
    [Route("api/practice/growth-messages")]
    public class GrowthMessagesController : ControllerBase
    {
        private readonly IGrowthMessageService _growthMessageService;

        public GrowthMessagesController(IGrowthMessageService growthMessageService)
        {
            _growthMessageService = growthMessageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(
            [FromBody] CreateGrowthMessageDto dto,
            CancellationToken cancellationToken)
        {
            var id = await _growthMessageService.CreateMessageAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(CreateMessage), new { id }, new { Id = id });
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleStatus(
            Guid id,
            [FromQuery] bool activate,
            CancellationToken cancellationToken)
        {
            await _growthMessageService.ToggleMessageStatusAsync(id, activate, cancellationToken);
            return NoContent();
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomMessage(
            [FromQuery] GrowthMessageType? type,
            [FromQuery] Guid? selectedStatementId,
            CancellationToken cancellationToken)
        {
            var message = await _growthMessageService.GetRandomMessageAsync(
                type ?? GrowthMessageType.Begin,
                selectedStatementId,
                cancellationToken);
            return Ok(message);
        }
    }
}
