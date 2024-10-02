namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask
{
    /// <summary>
    /// DTO para que el empleado actualice el estado de su tarea.
    /// </summary>
    public class UpdateTaskStateByEmployeeRequestDto
    {
        /// <summary>
        /// Identificador del nuevo estado de la tarea.
        /// </summary>
        public string NewWorkTaskStateId { get; set; } = string.Empty;
    }
}
