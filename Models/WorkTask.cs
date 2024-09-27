using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    /// <summary>
    /// Represents a task in the system, including its title, description, assigned user, and status.
    /// </summary>
    public class WorkTask
    {
        [Key]
        [Required]
        public Guid TaskId { get; set; }

        /// <summary>
        /// Title of the task.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        /// <summary>
        /// Description of the task (optional).
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// ID of the user assigned to the task.
        /// </summary>
        [Required]
        public string? AssignedUserId { get; set; }

        /// <summary>
        /// Navigation property for the assigned user.
        /// </summary>
        [ForeignKey("AssignedUserId")]
        public User? AssignedUserNavigation { get; set; }

        /// <summary>
        /// ID of the task status.
        /// </summary>
        [Required]
        public Guid WorkTaskStatusId { get; set; }

        /// <summary>
        /// Navigation property for the task status.
        /// </summary>
        [ForeignKey("WorkTaskStatusId")]
        public WorkTaskStatus? WorkTaskStatusNavigation { get; set; }

        /// <summary>
        /// ID of the user who created the task.
        /// </summary>
        [Required]
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Navigation property for the user who created the task.
        /// </summary>
        [ForeignKey("CreatedByUserId")]
        public User? CreatedByUserNavigation { get; set; }

        /// <summary>
        /// Date and time when the task was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
