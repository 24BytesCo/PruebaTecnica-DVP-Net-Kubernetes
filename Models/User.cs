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

        /// <summary>
        /// A collection of work tasks created by this user.
        /// </summary>
        public ICollection<WorkTask>? WorkTasks { get; set; }
    }
}
