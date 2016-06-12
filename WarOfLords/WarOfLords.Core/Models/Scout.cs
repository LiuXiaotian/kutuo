using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public class Scout : BattleUnit
    {
        public Scout(int id, string name):base(id, name)
        {
            this.MoveInterval = 100;
            this.MoveStepLength = 1;
            this.SightRange = 300;
        }

    }
}
