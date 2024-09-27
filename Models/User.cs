using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        public string? FirstName { get; set; } 

        [MaxLength(100)]
        public string? LastName { get; set; } 

        public ICollection<WorkTask>? AssignedWorkTasks { get; set; } 
    }
}