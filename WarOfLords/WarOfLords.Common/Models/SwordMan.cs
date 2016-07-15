using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public class SwordMan : Man
    {
        public SwordMan(int id, string name) : base(id, name)
        {
            this.CommandAbility = 10;
            this.SightRange = 50;
            this.AttackRange = 3;
            this.Damage = 30;
            this.DamageRange = 1;
            this.Defense = 10;
            this.DodgeRatio = 10;
            this.Health = 100;
            this.HitInterval = 1000;
            this.HitRatio = 90;
            this.IsMultiDamageTargets = false;
            this.IsRangeDamage = false;
            this.MaxDamageTargets = 1;
            this.MaxHealth = 100;
            this.MoveInterval = 500;
            this.MoveStepLength = 2;
        }
    }
}
