using Leaderboard.Application.ScoreEvent.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Leaderboard.Api.Test
{
    [TestClass]
    public class LeaderboardControllerTest
    {
        private IntegrationTestBuilder _builder = default!;
        private HttpClient _client = default!;
        private const string HEADER_CONTENT_TYPE = "application/json";
        [TestInitialize]
        public void Init()
        {
            _builder = new IntegrationTestBuilder();
            _client = _builder.CreateClient();
        }

      

        [TestMethod]
        public async Task PostScore_Success()
        {
            // Arrange
            var userId = $"test-user-{Guid.NewGuid():N}";
            var score = 123;
            var timestamp = DateTime.UtcNow;

            var postContent = new
            {
                UserId = userId,
                Score = score,
                Timestamp = timestamp
            };

            var content = new StringContent(JsonSerializer.Serialize(postContent), Encoding.UTF8, HEADER_CONTENT_TYPE);

            // Act: POST score
            var postResponse = await _client.PostAsync("api/leaderboard/score", content);
            postResponse.EnsureSuccessStatusCode();

            // Act: GET user score
            var getResponse = await _client.GetAsync($"api/leaderboard/user/{userId}/score");
            getResponse.EnsureSuccessStatusCode();

            var payload = await getResponse.Content.ReadAsStringAsync();
            var doc = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payload);

            // Assert
            Assert.IsNotNull(doc);
            Assert.IsTrue(doc.ContainsKey("score"));
            var returnedScore = doc["score"].GetDouble();
            Assert.AreEqual((double)score, returnedScore);
        }


        [TestMethod]
        public async Task GetLeaderboard_Success()
        {
            // Arrange: crear dos usuarios y varias puntuaciones
            var userA = $"user-A-{Guid.NewGuid():N}";
            var userB = $"user-B-{Guid.NewGuid():N}";

            var now = DateTime.UtcNow;

            // userA: 100 + 25 = 125
            var postA1 = new { UserId = userA, Score = 100, Timestamp = now };
            var postA2 = new { UserId = userA, Score = 25, Timestamp = now };

            // userB: 50
            var postB1 = new { UserId = userB, Score = 50, Timestamp = now };

            var contentA1 = new StringContent(JsonSerializer.Serialize(postA1), Encoding.UTF8, HEADER_CONTENT_TYPE);
            var contentA2 = new StringContent(JsonSerializer.Serialize(postA2), Encoding.UTF8, HEADER_CONTENT_TYPE);
            var contentB1 = new StringContent(JsonSerializer.Serialize(postB1), Encoding.UTF8, HEADER_CONTENT_TYPE);

            // Act: Postear puntuaciones
            var r1 = await _client.PostAsync("api/leaderboard/score", contentA1);
            if (!r1.IsSuccessStatusCode) Assert.Fail(await r1.Content.ReadAsStringAsync());

            var r2 = await _client.PostAsync("api/leaderboard/score", contentA2);
            if (!r2.IsSuccessStatusCode) Assert.Fail(await r2.Content.ReadAsStringAsync());

            var r3 = await _client.PostAsync("api/leaderboard/score", contentB1);
            if (!r3.IsSuccessStatusCode) Assert.Fail(await r3.Content.ReadAsStringAsync());

            // Act: Obtener leaderboard top=2
            var getLeaderboardResponse = await _client.GetAsync("api/leaderboard/leaderboard?top=2");
            getLeaderboardResponse.EnsureSuccessStatusCode();

            var payload = await getLeaderboardResponse.Content.ReadAsStringAsync();

            // Deserializar array de objetos { "userId": "...", "score": 123 }
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var entries = JsonSerializer.Deserialize<List<LeaderboardItemDto>>(payload, options);

            Assert.IsNotNull(entries, $"Payload inesperado: {payload}");
            Assert.AreEqual(2, entries!.Count, $"Se esperaba exactamente 2 entradas en el leaderboard. Payload:\n{payload}");

            // Convertir a diccionario para aserciones por usuario
            var leaderboard = entries.ToDictionary(e => e.UserId, e => e.Score);

            // Comprobar puntajes agregados
            Assert.IsTrue(leaderboard.ContainsKey(userA), $"Leaderboard no contiene userA. Payload:\n{payload}");
            Assert.IsTrue(leaderboard.ContainsKey(userB), $"Leaderboard no contiene userB. Payload:\n{payload}");

            Assert.AreEqual(125d, leaderboard[userA], 0.0001, "Puntaje agregado de userA incorrecto");
            Assert.AreEqual(50d, leaderboard[userB], 0.0001, "Puntaje agregado de userB incorrecto");
        }

        [TestMethod]
        public async Task GetUserScore_ReturnsCorrectScore()
        {
            // Arrange
            var userId = $"user-{Guid.NewGuid():N}";
            var now = DateTime.UtcNow;

            // Publicar varias puntuaciones para el mismo usuario
            var post1 = new { UserId = userId, Score = 10, Timestamp = now };
            var post2 = new { UserId = userId, Score = 15, Timestamp = now };

            var content1 = new StringContent(JsonSerializer.Serialize(post1), Encoding.UTF8, HEADER_CONTENT_TYPE);
            var content2 = new StringContent(JsonSerializer.Serialize(post2), Encoding.UTF8, HEADER_CONTENT_TYPE);

            var p1 = await _client.PostAsync("api/leaderboard/score", content1);
            if (!p1.IsSuccessStatusCode) Assert.Fail(await p1.Content.ReadAsStringAsync());

            var p2 = await _client.PostAsync("api/leaderboard/score", content2);
            if (!p2.IsSuccessStatusCode) Assert.Fail(await p2.Content.ReadAsStringAsync());

            // Act: solicitar score por usuario
            var getResponse = await _client.GetAsync($"api/leaderboard/user/{userId}/score");
            getResponse.EnsureSuccessStatusCode();

            var payload = await getResponse.Content.ReadAsStringAsync();
            var doc = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payload);

            // Assert: estructura { user_id: "...", score: X }
            Assert.IsNotNull(doc, $"Payload inválido: {payload}");
            Assert.IsTrue(doc.ContainsKey("user_id"), "Respuesta no contiene 'user_id'");
            Assert.IsTrue(doc.ContainsKey("score"), "Respuesta no contiene 'score'");

            var returnedUser = doc["user_id"].GetString();
            var returnedScore = doc["score"].GetDouble();

            Assert.AreEqual(userId, returnedUser, "El user_id devuelto no coincide");
            Assert.AreEqual(25d, returnedScore, 0.0001, "El score devuelto no coincide con la suma de las puntuaciones");
        }

    }
}
