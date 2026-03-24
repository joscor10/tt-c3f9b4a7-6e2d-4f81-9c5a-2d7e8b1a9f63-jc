using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Application.ScoreEvent.Queries
{
    public record GetUserScoreQuery
    (string UserId) : IRequest<double?>;
}
