using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Application.ScoreEvent.Commands
{
    public record AddScoreCommand
    (string UserId,int Score, DateTime Timestamp) :IRequest;
}
