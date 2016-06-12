using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public interface IMedicalCapability
    {
        int Recover { get; set; }
        int RecoverInterval { get; set; }
        int RecoverRatio { get; set; }
        bool IsRangeRecover { get; set; }
        int RecoverRange { get; set; }

        bool IsMultiRecoverTargets { get; set; }
        int MaxRecoverTargets { get; set; }
    }
}
