using Leaderboard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Domain.Ports
{
    public interface IScoreRepository:IGenericRepository<ScoreEvent>
    {
        Task SaveAsync(ScoreEvent score);
        Task<IEnumerable<ScoreEvent>> GetByDateRange(DateTime from, DateTime to);
    }
}
