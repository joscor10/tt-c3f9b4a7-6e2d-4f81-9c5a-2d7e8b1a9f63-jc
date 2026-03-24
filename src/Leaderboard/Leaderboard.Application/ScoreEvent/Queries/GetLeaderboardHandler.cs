using Leaderboard.Application.ScoreEvent.DTOs;
using Leaderboard.Domain.Ports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Application.ScoreEvent.Queries
{
    public class GetLeaderboardHandler : IRequestHandler<GetLeaderboardQuery, IEnumerable<LeaderboardItemDto>>
    {
        private readonly ILeaderboardCache _cache;

        public GetLeaderboardHandler(ILeaderboardCache cache)
        {
            _cache = cache;
        }
        public async Task<IEnumerable<LeaderboardItemDto>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
        {
            var data = await _cache.GetTopAsync(request.Top);

            return data.Select(x => new LeaderboardItemDto
            {
                UserId = x.Key,
                Score = x.Value
            }).ToList();
        }
    }
}
