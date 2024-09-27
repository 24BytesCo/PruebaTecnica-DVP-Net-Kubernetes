using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.WorkTaskService
{
    public interface IWorkTaskService
    {
        /// <summary>
        /// Creates a new work task.
        /// </summary>
        /// <param name="taskCreateDto">DTO with the information required to create a task.</param>
        /// <returns>Returns a GenericResponse with the newly created task.</returns>
        Task<GenericResponse<NewCreateWorkTaskDto>> CreateTaskAsync(TaskCreateRequestDto taskCreateDto);
        
        /// <summary>
        /// Retrieves all the tasks.
        /// </summary>
        /// <returns>Returns a list of all tasks.</returns>
        Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> GetAllTasksAsync();

        /// <summary>
        /// Recovers all tasks assigned to the logged in user.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> GetAllTheTasksAssignedToMe();

        /// <summary>
        /// Retrieves a specific task by ID.
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <returns>Returns the task if found, otherwise an error message.</returns>
        Task<GenericResponse<WorkTask>> GetTaskByIdAsync(Guid id);

        /// <summary>
        /// Updates a task by its ID.
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <param name="taskUpdateDto">DTO containing the updated task details.</param>
        /// <returns>Returns the updated task or an error if not found.</returns>
        Task<GenericResponse<WorkTask>> UpdateTaskAsync(Guid id, TaskUpdateRequestDto taskUpdateDto);

        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <returns>Returns a success message or an error if the task was not found.</returns>
        Task<GenericResponse<bool>> DeleteTaskAsync(Guid id);
    }
}
