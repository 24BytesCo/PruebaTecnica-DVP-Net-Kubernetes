using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica_DVP_Net_Kubernetes.Models
{
    public class WorkTaskStatus
    {
        [Key]
        [Required]
        public Guid TaskStatusId { get; set; } 

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; } 

        [MaxLength(200)]
        public string? Description { get; set; } 

        [Required]
        [MaxLength(10)]
        public string? Code { get; set; } 
        public ICollection<WorkTask>? WorkTask { get; set; } 
    }
}