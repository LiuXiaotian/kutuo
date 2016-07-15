using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public class WeaponOperator : Man
    {
        private bool isRegistered = false;

        public WeaponOperator(int id, string name) : base(id, name)
        {
            this.Id = id;
            this.CommandAbility = 10;
            this.SightRange = 100;
            this.MoveInterval = 500;
            this.MoveStepLength = 2;
        }

        public bool Registered
        {
            get
            {
                return isRegistered;
            }
        }

        public override int CommandAbility
        {
            get; set;
        }

        public override int Id
        {
            get; set;
        }

        public override MapVertex Position
        {
            get; set;
        }

        public override int SightRange
        {
            get; set;
        }

        public override int MoveInterval
        {
            get; set;
        }

        public override int MoveStepLength
        {
            get; set;
        }

        public bool Register()
        {
            if(Registered)
            {
                throw new InvalidOperationException();
            }
            isRegistered = true;
            return true;
        }

        public void Leave()
        {
            isRegistered = false;
        }
    }
}
