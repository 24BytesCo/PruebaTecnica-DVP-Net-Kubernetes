using PruebaTecnica_DVP_Net_Kubernetes.Dtos.User;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTaskState;

namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask
{
    public class GetAllTheTasksAssignedToMeResponseDto
    {
        public string? WorkTaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public UserMinimalResponseDto? UserAssignedObj { get; set; }
        public UserMinimalResponseDto? UserByCreatedObj { get; set; }
        public WorkTaskStateMinimalResponseDto? WorkTaskStateObj { get; set; }
    }

}
