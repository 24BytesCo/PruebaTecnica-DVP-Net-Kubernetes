using PruebaTecnica_DVP_Net_Kubernetes.Dtos.User;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTaskState;

namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask
{
    public class TasksAssignedToMeResponseDto
    {
        public string? WorkTaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public UserMinimalResponseDto? UserAssigned { get; set; }
        public UserMinimalResponseDto? UserByCreated { get; set; }
        public WorkTaskStateMinimalResponseDto? WorkTaskState { get; set; }
    }
}
