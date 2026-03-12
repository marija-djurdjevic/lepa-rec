using AngularNetBase.Practice.Dtos.AffirmationValues;
using AngularNetBase.Practice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AngularNetBase.API.Controllers
{
    [ApiController]
    [Route("api/practice/affirmation-values")]
    public class AffirmationValuesController : ControllerBase
    {
        private readonly IAffirmationValueService _affirmationValueService;

        public AffirmationValuesController(IAffirmationValueService affirmationValueService)
        {
            _affirmationValueService = affirmationValueService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAffirmationValue(
            [FromBody] CreateAffirmationValueDto dto,
            CancellationToken cancellationToken)
        {
            var id = await _affirmationValueService.CreateAffirmationValueAsync(dto, cancellationToken);

            return CreatedAtAction(nameof(CreateAffirmationValue), new { id }, new { Id = id });
        }

        [HttpPost("{id}/statements")]
        public async Task<IActionResult> AddStatement(
            Guid id,
            [FromBody] AddStatementDto dto,
            CancellationToken cancellationToken)
        {
            var statementId = await _affirmationValueService.AddStatementAsync(id, dto, cancellationToken);

            return Ok(new { StatementId = statementId });
        }

        [HttpPatch("{id}/statements/{statementId}/status")]
        public async Task<IActionResult> ToggleStatementStatus(
            Guid id,
            Guid statementId,
            [FromQuery] bool activate,
            CancellationToken cancellationToken)
        {
            await _affirmationValueService.ToggleStatementStatusAsync(id, statementId, activate, cancellationToken);

            return NoContent();
        }

        [HttpGet("random-statements")]
        public async Task<IActionResult> GetRandomPrimerStatements(CancellationToken cancellationToken)
        {
            var statements = await _affirmationValueService.GetPrimerStatementsAsync(cancellationToken);

            return Ok(statements);
        }
    }
}
