using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.User
{
    public class UserMinimalResponseDto
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NameCompleted { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleCode { get; set; }
    }
}