using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public abstract class Man : BattleUnit, ICommandCapability
    {
        private object moveLock = new object();

        public virtual int CommandAbility { get; set; }

        public Man(int id, string name) : base(id, name)
        {
            this.CommandAbility = 10;
        }
    }
}
