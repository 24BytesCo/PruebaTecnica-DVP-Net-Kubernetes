namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask
{
    public class UpdateStateAndUserAssignRequestDto
    {
        public string? NewUserAssignedId { get; set; }
        public string? NewWorkTaskStateId { get; set; }
    }
}
