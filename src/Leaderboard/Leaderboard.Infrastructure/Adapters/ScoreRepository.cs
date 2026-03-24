using Leaderboard.Domain.Entities;
using Leaderboard.Domain.Ports;
using Leaderboard.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Infrastructure.Adapters
{
    public class ScoreRepository : GenericRepository<ScoreEvent>, IScoreRepository
    {
        public ScoreRepository(PersistenceContext context) : base(context)
        {
        }

        public Task<IEnumerable<ScoreEvent>> GetByDateRange(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync(ScoreEvent score)
        {
           await AddAsync(score);
        }
    }
}
