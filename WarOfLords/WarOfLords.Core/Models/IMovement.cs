using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public interface IMovementCapability
    {
        MapVertex Position { get; set; }
        int MoveInterval { get; set; }
        int MoveStepLength { get; set; }
        Task<int> MoveTo(MapVertex pos);
        int SightRange { get; set; }
    }
}
