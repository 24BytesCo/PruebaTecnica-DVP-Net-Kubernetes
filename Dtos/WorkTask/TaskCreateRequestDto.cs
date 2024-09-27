namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask
{
    public class TaskCreateRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? UserAssignedId { get; set; }
    }

}
