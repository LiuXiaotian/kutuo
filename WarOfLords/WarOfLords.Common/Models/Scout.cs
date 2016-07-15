using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public class Scout : BattleUnit
    {
        public Scout(int id, string name):base(id, name)
        {
            this.MoveInterval = 500;
            this.MoveStepLength = 5;
            this.SightRange = 300;
        }

    }
}
