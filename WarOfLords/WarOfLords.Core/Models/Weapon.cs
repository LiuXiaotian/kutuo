using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public abstract class Weapon : BattleUnit
    {
        private List<WeaponOperator> operators = new List<WeaponOperator>();
        private int hitInterval;

        public Weapon(int id, string name) : base(id, name)
        {
            
        }

        public IEnumerable<WeaponOperator> Operators {get; }
        public virtual int RequiredOperatorCount { get; set; }
        public virtual int CurrentOperatorCount
        {
            get
            {
                return operators.Count;
            }
        }

        public override int HitInterval
        {
            get
            {
                if (CurrentOperatorCount > 0)
                    return hitInterval * RequiredOperatorCount / CurrentOperatorCount;
                else return hitInterval;
            }

            set
            {
                hitInterval = value;
            }
        }

        public override int Damage
        {
            get
            {
                if (CurrentOperatorCount > 0)
                    return base.Damage;
                else return 0;
            }

            set
            {
                base.Damage = value;
            }
        }

        public virtual bool IsQualifiedOperator(WeaponOperator operater)
        {
            return true;
        }

        public virtual bool AddOperator(WeaponOperator operater)
        {
            if (CurrentOperatorCount >= RequiredOperatorCount) return false;
            if (IsQualifiedOperator(operater))
            {
                this.operators.Add(operater);
                operater.Register();
                return true;
            }
            else return false;
        }

        public virtual void AddOperators(IEnumerable<WeaponOperator> operators)
        {
            operators.Where(_ => !_.Registered).ToList().ForEach(_ => AddOperator(_));
        }
    }
}
