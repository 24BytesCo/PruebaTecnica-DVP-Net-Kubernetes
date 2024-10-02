using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.taskStates;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.TaskState
{
    public interface ITaskStateService
    {
        /// <summary>
        /// Get all task states
        /// </summary>
        /// <returns>Returns a GenericResponse with the all task states.</returns>
        Task<GenericResponse<List<TaskStatesResponseDto>>> GetAllTaskStates();
    }
}
