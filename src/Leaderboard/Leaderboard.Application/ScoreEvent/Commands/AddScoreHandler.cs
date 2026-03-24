using Leaderboard.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Application.ScoreEvent.Commands
{
    public class AddScoreHandler : IRequestHandler<AddScoreCommand>
    {
        private readonly AddScoreService _addScoreService;

        public AddScoreHandler(AddScoreService addScoreService)
        {
            _addScoreService = addScoreService;
        }
        public async Task Handle(AddScoreCommand request, CancellationToken cancellationToken)
        {
            await _addScoreService.Execute(request.UserId,request.Score, DateTime.SpecifyKind(request.Timestamp, DateTimeKind.Utc) );
           
        }
    }
}
