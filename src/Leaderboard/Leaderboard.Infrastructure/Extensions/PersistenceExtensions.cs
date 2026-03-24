using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Infrastructure.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection svc, IConfiguration config)
        {
            return svc;
        }
    }
}
