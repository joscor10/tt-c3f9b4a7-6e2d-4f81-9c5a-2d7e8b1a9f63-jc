using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Application.ScoreEvent.DTOs
{
    public class LeaderboardItemDto
    {
        public string UserId { get; set; }
        public double Score { get; set; }
    }
}
