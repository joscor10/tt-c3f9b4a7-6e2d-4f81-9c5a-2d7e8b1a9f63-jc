using Leaderboard.Domain.Ports;
using Leaderboard.Infrastructure.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Infrastructure.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection svc, IConfiguration config)
        {

            svc.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            svc.AddScoped<ILeaderboardCache, LeaderboardCache>();
            svc.AddTransient<IScoreRepository, ScoreRepository>();
            return svc;
        }
    }
}
