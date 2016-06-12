using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public class Trebuchet : Weapon
    {
        public Trebuchet(int id, string name) : base(id, name)
        {
            this.SightRange = 100;
            this.AttackRange = 50;
            this.Damage = 120;
            this.DamageRange = 5;
            this.Defense = 10;
            this.DodgeRatio = 0;
            this.Health = 500;
            this.HitInterval = 60000;
            this.HitRatio = 70;
            this.IsMultiDamageTargets = true;
            this.IsRangeDamage = true;
            this.MaxDamageTargets = 5;
            this.MaxHealth = 500;
            this.MoveInterval = 800;
            this.MoveStepLength = 2;

            this.RequiredOperatorCount = 5;
        }

    }
}
