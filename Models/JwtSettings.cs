using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    public class JwtSettings
    {
        public string? Secret { get; set; }
        public int Expires { get; set; }
    }

}