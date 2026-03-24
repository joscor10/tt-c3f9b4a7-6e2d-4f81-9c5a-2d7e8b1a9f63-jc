using Leaderboard.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Domain.Entities
{
    public class ScoreEvent:EntityBase<Guid>
    {
        public string UserId { get; set; }
        public int Score { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
