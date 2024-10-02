using Microsoft.AspNetCore.Identity;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    public class Role: IdentityRole
    {
        public string? Code { get; set; }
    }
}
