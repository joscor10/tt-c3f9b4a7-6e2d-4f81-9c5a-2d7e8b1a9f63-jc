using Leaderboard.Domain.Ports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Application.ScoreEvent.Queries
{
    public class GetUserScoreHandler : IRequestHandler<GetUserScoreQuery, double?>
    {
        private readonly ILeaderboardCache _cache;

        public GetUserScoreHandler(ILeaderboardCache cache)
        {
            _cache = cache;
        }
        public async Task<double?> Handle(GetUserScoreQuery request, CancellationToken cancellationToken)
        {
            return await _cache.GetUserScoreAsync(
             request.UserId
             
         );
        }
    }
}
