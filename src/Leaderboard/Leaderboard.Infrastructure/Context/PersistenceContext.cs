

using Leaderboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Leaderboard.Infrastructure.Context
{
    public class PersistenceContext : DbContext
    {

        private readonly IConfiguration _config;

        public PersistenceContext(DbContextOptions<PersistenceContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                return;
            }


            modelBuilder.Entity<ScoreEvent>(entity =>
            {
                entity.ToTable("scores");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .IsRequired();

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
