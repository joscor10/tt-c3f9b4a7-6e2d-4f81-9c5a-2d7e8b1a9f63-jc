using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Domain.Ports
{
    public interface ILeaderboardCache
    {
        Task IncrementScoreAsync(string userId, int score, DateTime date);
        Task<Dictionary<string, double>> GetTopAsync(int top);
        Task<double?> GetUserScoreAsync(string userId);
    }
}
