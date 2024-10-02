using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    public class Role: IdentityRole
    {
        [MaxLength(50)]
        public string? Code { get; set; }
    }
}
