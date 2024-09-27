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
        public Guid TaskId { get; set; }

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
        public string? AssignedToUserId { get; set; } // Foreign key to User

        /// <summary>
        /// Gets or sets the identifier of the user who created the task.
        /// This is a foreign key to the User entity.
        /// </summary>
        [Required]
        public string? CreatedByUserId { get; set; } // Foreign key to User

        /// <summary>
        /// Navigation property for the user who created the task.
        /// </summary>
        [ForeignKey("CreatedByUserId")]
        public User? CreatedByUserIdNavigation { get; set; } // Navigation property for User

        /// <summary>
        /// Gets or sets the identifier of the task's status.
        /// </summary>
        [Required]
        public Guid TaskStatusId { get; set; } // Foreign key to WorkTaskStatus

        /// <summary>
        /// Navigation property for the task's status.
        /// </summary>
        [ForeignKey("TaskStatusId")]
        public WorkTaskStatus? WorkTaskStatusNavigation { get; set; } // Navigation property for WorkTaskStatus
    }
}
