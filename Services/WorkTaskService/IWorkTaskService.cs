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
        Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> GetAllTasksAsync(int page, int pageSize);

        /// <summary>
        /// Recovers all tasks assigned to the logged in user.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> GetAllTheTasksAssignedToMe(int page = 1, int pageSize = 6);

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
        Task<GenericResponse<bool>> UpdateTaskAsync(Guid id, TaskUpdateRequestDto taskUpdateDto);

        Task<GenericResponse<bool>> UpdateTaskStateAndUserAssign(Guid id, UpdateStateAndUserAssignRequestDto requestDto);

        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <returns>Returns a success message or an error if the task was not found.</returns>
        Task<GenericResponse<bool>> DeleteTaskAsync(string id);

        /// <summary>
        /// Obtiene una tarea por su identificador único.
        /// </summary>
        /// <param name="id">Identificador único de la tarea.</param>
        /// <param name="requestDto">requestDto.</param>
        /// <returns>La entidad de la tarea si se encuentra; de lo contrario, null.</returns>
        Task<GenericResponse<bool>> UpdateTaskStateByEmployeeAsync(Guid id, UpdateTaskStateByEmployeeRequestDto requestDto);


        /// <summary>
        /// Recovers all tasks found.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> SearchTasksDynamic(string query, int pageNumber = 1, int pageSize = 6);
    }
}
