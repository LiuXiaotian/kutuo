using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public class BowMan : Man
    {
        public BowMan(int id, string name):base(id, name)
        {
            this.CommandAbility = 10;
            this.SightRange = 50;
            this.AttackRange = 20;
            this.Damage = 20;
            this.DamageRange = 1;
            this.Defense = 5;
            this.DodgeRatio = 30;
            this.Health = 100;
            this.HitInterval = 3000;
            this.HitRatio = 60;
            this.IsMultiDamageTargets = false;
            this.IsRangeDamage = false;
            this.MaxDamageTargets = 1;
            this.MaxHealth = 100;
            this.MoveInterval = 500;
            this.MoveStepLength = 2;
        }

       

    }
}
