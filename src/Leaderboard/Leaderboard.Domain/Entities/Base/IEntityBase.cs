using System;
using System.Collections.Generic;
using System.Text;

namespace Leaderboard.Domain.Entities.Base
{
    public interface IEntityBase<T>
    {
        T Id { get; set; }
    }
}
