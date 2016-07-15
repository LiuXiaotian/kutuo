using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public interface IFightCapability
    {
        int Damage { get; set; }
        int HitInterval { get; set; }
        int HitRatio { get; set; } 
        int AttackRange { get; set; }

        bool IsRangeDamage { get; set; }
        int DamageRange { get; set; }

        bool IsMultiDamageTargets { get; set; }
        int MaxDamageTargets { get; set; }

        int TotalDamageToEnemies { get; set; }

        int TotalKilled { get; set; }
    }
}
