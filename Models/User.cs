using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    /// <summary>
    /// Represents a user in the system. Inherits from IdentityUser and includes additional properties such as FirstName and LastName.
    /// The user can have tasks assigned to them and can also create tasks.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// The first name of the user.
        /// </summary>
        [MaxLength(100)]
        public string? FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        [MaxLength(100)]
        public string? LastName { get; set; }


        // Tareas asignadas al usuario
        public ICollection<WorkTask>? AssignedWorkTasks { get; set; }

        // Tareas creadas por el usuario
        //CreatedByUserNavigation
        public ICollection<WorkTask>? CreatedWorkTasks { get; set; }
       
    }
}
