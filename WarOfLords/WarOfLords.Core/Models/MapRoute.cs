using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarOfLords.Core.Models
{
    public class MapRoute
    {
        public virtual MapVertex StartPoint {get;set;}

        public virtual MapVertex EndPoint { get; set; }

    }
}
