using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    public class WorkTask
    {
        [Key]
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string? AssignedUserId { get; set; }

        [ForeignKey("AssignedUserId")]
        public User? AssignedUserNavigation { get; set; }

        [Required]
        public Guid TaskStatusId { get; set; }

        [ForeignKey("WorkTaskStatusId")]
        public WorkTaskStatus? WorkTaskStatusNavigation { get; set; }
    }
}