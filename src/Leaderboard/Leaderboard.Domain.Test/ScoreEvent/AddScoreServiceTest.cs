using Leaderboard.Domain.Ports;
using Leaderboard.Domain.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Domain.Test.ScoreEvent
{
    [TestClass]
    public class AddScoreServiceTest
    {
        IScoreRepository _repo= default!;
        ILeaderboardCache _cache= default!;

        [TestInitialize]
        public void Init()
        {
            _repo = Substitute.For<IScoreRepository>();
            _cache = Substitute.For<ILeaderboardCache>();
        }

        [TestMethod]
        public async Task Execute_ValidInputs_SavesEventAndIncrementsCache()
        {
            // Arrange
            var userId = "user-123";
            var score = 150;
            var timestamp = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

            _repo.SaveAsync(Arg.Any<Entities.ScoreEvent>()).Returns(Task.CompletedTask);
            _cache.IncrementScoreAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<DateTime>()).Returns(Task.CompletedTask);

            var service = new AddScoreService(_repo, _cache);

            // Act
            await service.Execute(userId, score, timestamp);

            // Assert: se guardó un ScoreEvent con los valores esperados y un Id generado
            _repo.Received(1).SaveAsync(Arg.Is<Entities.ScoreEvent>(e =>
                e.UserId == userId &&
                e.Score == score &&
                e.Timestamp == timestamp &&
                e.Id != Guid.Empty));

            // Assert: cache incrementada con los mismos parámetros
            _cache.Received(1).IncrementScoreAsync(userId, score, timestamp);
        }
    }
}
