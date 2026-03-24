using Leaderboard.Infrastructure.Context;
using Leaderboard.Domain.Ports;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leaderboard.Api.Test
{
    internal class IntegrationTestBuilder : WebApplicationFactory<Program>
    {
        readonly Guid _id;

        public Guid Id => this._id;

        public IntegrationTestBuilder()
        {
            _id = Guid.NewGuid();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var rootDb = new InMemoryDatabaseRoot();

         
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            builder.ConfigureServices(services =>
            {

              
                services.RemoveAll(typeof(DbContextOptions<PersistenceContext>));
                services.RemoveAll(typeof(PersistenceContext));

               
                services.AddDbContext<PersistenceContext>(options =>
                    options.UseInMemoryDatabase("Testing", rootDb)
                           .UseInternalServiceProvider(efServiceProvider));

              
                services.RemoveAll(typeof(StackExchange.Redis.IConnectionMultiplexer));
                services.RemoveAll(typeof(ILeaderboardCache));

              
                services.AddMemoryCache();
                services.AddSingleton<ILeaderboardCache, MemoryLeaderboardCache>();
            });

            return base.CreateHost(builder);
        }
    }

    // Implementación simple en memoria para pruebas
    internal class MemoryLeaderboardCache : ILeaderboardCache
    {
        // key (yyyy-MM-dd) -> (userId -> score)
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, double>> _store
            = new(StringComparer.Ordinal);

        public Task<Dictionary<string, double>> GetTopAsync(int top)
        {
            var aggregate = new Dictionary<string, double>(StringComparer.Ordinal);

            foreach (var day in _store.Values)
            {
                foreach (var kv in day)
                {
                    if (!aggregate.ContainsKey(kv.Key))
                        aggregate[kv.Key] = 0;
                    aggregate[kv.Key] += kv.Value;
                }
            }

            var result = aggregate
                .OrderByDescending(x => x.Value)
                .Take(top)
                .ToDictionary(x => x.Key, x => x.Value);

            return Task.FromResult(result);
        }

        public Task<double?> GetUserScoreAsync(string userId)
        {
            double total = 0;
            foreach (var day in _store.Values)
            {
                if (day.TryGetValue(userId, out var s))
                    total += s;
            }
            return Task.FromResult<double?>(total);
        }

        public Task IncrementScoreAsync(string userId, int score, DateTime date)
        {
            var key = date.ToString("yyyy-MM-dd");
            var dayDict = _store.GetOrAdd(key, _ => new ConcurrentDictionary<string, double>(StringComparer.Ordinal));
            dayDict.AddOrUpdate(userId, score, (_, old) => old + score);
            return Task.CompletedTask;
        }
    }
}