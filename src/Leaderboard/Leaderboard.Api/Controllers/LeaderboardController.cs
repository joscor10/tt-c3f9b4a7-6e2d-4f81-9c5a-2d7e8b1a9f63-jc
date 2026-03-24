using Leaderboard.Application.ScoreEvent.Commands;
using Leaderboard.Application.ScoreEvent.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Leaderboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaderboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        [HttpPost("score")]
        public async Task<IActionResult> AddScore([FromBody] AddScoreCommand request)
        {
          

            await _mediator.Send(request);

            return Ok();
        }

      
        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int top )
        {
            var result = await _mediator.Send(new GetLeaderboardQuery( top ));

            return Ok(result);
        }

       
        [HttpGet("user/{id}/score")]
        public async Task<IActionResult> GetUserScore(string id)
        {
            var result = await _mediator.Send(new GetUserScoreQuery(id));

            return Ok(new { user_id = id, score = result });
        }
    }
}
