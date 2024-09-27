using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    /// <summary>
    /// Represents the status of a work task, including its name, description, and associated code.
    /// </summary>
    public class WorkTaskStatus
    {
        /// <summary>
        /// The unique identifier for the task status.
        /// </summary>
        [Key]
        [Required]
        public string? WorkTaskStatusId { get; set; } 

        /// <summary>
        /// The name of the task status (e.g., Pending, In Progress, Completed).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; } 

        /// <summary>
        /// A description of the task status (optional).
        /// </summary>
        [MaxLength(200)]
        public string? Description { get; set; } 

        /// <summary>
        /// A unique code that represents the task status.
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string? Code { get; set; }

        /// <summary>
        /// A collection of work tasks that have this status.
        /// TaskStatusNavigation 
        /// </summary>
        public ICollection<WorkTask>? WorkTask { get; set; } 
    }
}
