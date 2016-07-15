using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Common.Models
{
    public class MedicalMan : Man, IMedicalCapability
    {
        public MedicalMan(int id, string name) : base(id, name)
        {
            this.CommandAbility = 10;
            this.IsMultiRecoverTargets = false;
            this.IsRangeRecover = false;
            this.MaxRecoverTargets = 1;
            this.MoveInterval = 600;
            this.MoveStepLength = 3;
            this.Recover = 30;
            this.RecoverInterval = 180000;
            this.RecoverRange = 1;
            this.RecoverRatio = 100;
            this.SightRange = 50;
        }

        public bool IsMultiRecoverTargets
        {
            get; set;
        }

        public bool IsRangeRecover
        {
            get; set;
        }

        public int MaxRecoverTargets
        {
            get; set;
        }

        public int Recover
        {
            get; set;
        }

        public int RecoverInterval
        {
            get; set;
        }

        public int RecoverRange
        {
            get; set;
        }

        public int RecoverRatio
        {
            get; set;
        }

    }
}
