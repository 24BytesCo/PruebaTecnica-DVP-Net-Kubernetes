namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask
{
    public class NewCreateWorkTaskDto
    {
        public string? WorkTaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? WorkTaskStatusId { get; set; }
        public string? WorkTaskStatusName { get; set; }
        public string? WorkTaskStatusCode { get; set; }
        public string? UserAssignedId { get; set; }
        public string? UserAssignedFirsName { get; set; }
        public string? UserAssignedLastName { get; set; }
    }
}
