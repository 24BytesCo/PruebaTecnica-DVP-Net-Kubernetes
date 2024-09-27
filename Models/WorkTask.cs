using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    /// <summary>
    /// Represents a task in the system.
    /// </summary>
    public class WorkTask
    {
        /// <summary>
        /// Gets or sets the unique identifier for the task.
        /// </summary>
        [Key]
        [Required]
        public string? TaskId { get; set; }

        /// <summary>
        /// Gets or sets the title of the task.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user to whom the task is assigned.
        /// This is a foreign key to the User entity.
        /// </summary>
        [Required]
        public string? AssignedToUserId { get; set; }

        /// <summary>
        /// Navigation property for the user to whom the task is assigned.
        /// </summary>
        [ForeignKey("AssignedToUserId")]
        [InverseProperty("AssignedWorkTasks")] // Indica que esto se relaciona con AssignedWorkTasks en User
        public User? AssignedToUserNavigation { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the task.
        /// This is a foreign key to the User entity.
        /// </summary>
        [Required]
        public string? CreatedByUserId { get; set; }

        /// <summary>
        /// Navigation property for the user who created the task.
        /// </summary>
        [ForeignKey("CreatedByUserId")]
        [InverseProperty("CreatedWorkTasks")] // Indica que esto se relaciona con AssignedWorkTasks en User
        public User? CreatedByUserNavigation { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the task's status.
        /// </summary>
        [Required]
        public string? WorkTaskStatusId { get; set; }

        /// <summary>
        /// Navigation property for the task's status.
        /// </summary>
        [ForeignKey("WorkTaskStatusId")]
        [InverseProperty("WorkTask")] // Indica que esto se relaciona con WorkTask en WorkTaskStatus
        public WorkTaskStatus? WorkTaskStatusNavigation { get; set; }
    }
}
