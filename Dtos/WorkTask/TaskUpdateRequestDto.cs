﻿namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask
{
    public class TaskUpdateRequestDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? NewUserAssignedId { get; set; }
        public string? NewWorkTaskStateId { get; set; }
    }
}
