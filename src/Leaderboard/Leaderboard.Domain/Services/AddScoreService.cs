using Leaderboard.Domain.Entities;
using Leaderboard.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Domain.Services
{
    [DomainService]
    public class AddScoreService
    {
        private readonly IScoreRepository _repo;
        private readonly ILeaderboardCache _cache;

        public AddScoreService(IScoreRepository repo, ILeaderboardCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task Execute(string userId, int score, DateTime timestamp)
        {
            var eventScore = new ScoreEvent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Score = score,
                Timestamp = timestamp
            };

         
            await _repo.SaveAsync(eventScore);

        
            await _cache.IncrementScoreAsync(userId, score, timestamp);
        }
    }
}
