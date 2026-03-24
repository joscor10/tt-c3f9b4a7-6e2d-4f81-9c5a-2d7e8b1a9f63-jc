using Leaderboard.Domain.Ports;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Infrastructure.Adapters
{
    public class LeaderboardCache : ILeaderboardCache
    {
        private readonly IDatabase _db;
        private readonly IConfiguration _config;
        public LeaderboardCache(IConnectionMultiplexer redis, IConfiguration config)
        {
            _db = redis.GetDatabase();
            _config= config;
        }
        public async Task<Dictionary<string, double>> GetTopAsync(int top)
        {
            var result = new Dictionary<string, double>();
            int days = int.Parse(_config.GetSection("Leaderboard").GetSection("DefaultDays").Value ?? "0");
            for (int i = 0; i < days; i++)
            {
                var key = $"leaderboard:{DateTime.UtcNow.AddDays(-i):yyyy-MM-dd}";

                var entries = await _db.SortedSetRangeByRankWithScoresAsync(
                    key, 0, top - 1, Order.Descending);

                foreach (var entry in entries)
                {
                    var userId = entry.Element.ToString();
                    var score = entry.Score;

                    if (!result.ContainsKey(userId))
                        result[userId] = 0;

                    result[userId] += score;
                }
            }

            return result
                .OrderByDescending(x => x.Value)
                .Take(top)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<double?> GetUserScoreAsync(string userId)
        {
            double total = 0;
            int days = int.Parse(_config.GetSection("Leaderboard").GetSection("DefaultDays").Value??"0");

            for (int i = 0; i < days; i++)
            {
                var key = $"leaderboard:{DateTime.UtcNow.AddDays(-i):yyyy-MM-dd}";

                var score = await _db.SortedSetScoreAsync(key, userId);

                if (score.HasValue)
                    total += score.Value;
            }

            return total;
        }

        public async Task IncrementScoreAsync(string userId, int score, DateTime date)
        {
            var key = $"leaderboard:{date:yyyy-MM-dd}";

            await _db.SortedSetIncrementAsync(key, userId, score);
        }
    }
}
