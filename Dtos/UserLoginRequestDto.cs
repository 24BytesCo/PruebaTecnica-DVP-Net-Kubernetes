using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos
{
    public class UserLoginRequestDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}