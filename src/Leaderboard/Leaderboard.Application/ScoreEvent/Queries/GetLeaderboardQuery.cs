using Leaderboard.Application.ScoreEvent.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Application.ScoreEvent.Queries
{
    public record GetLeaderboardQuery
    (int Top):IRequest<IEnumerable<LeaderboardItemDto>>;
    
}
