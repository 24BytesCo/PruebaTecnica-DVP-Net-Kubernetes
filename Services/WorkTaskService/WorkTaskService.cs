using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Token;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.WorkTaskService
{
    /// <summary>
    /// Service to manage work tasks, including creating, updating, retrieving, and deleting tasks.
    /// </summary>
    /// <remarks>
    /// Servicio para gestionar las tareas de trabajo, incluyendo la creación, actualización, recuperación y eliminación de tareas.
    /// </remarks>
    public class WorkTaskService : IWorkTaskService
    {
        private readonly AppDbContext _context;
        private readonly IUserSesion _userSesion;

        /// <summary>
        /// Constructor for WorkTaskService.
        /// </summary>
        /// <param name="context">Database context to be injected.</param>
        public WorkTaskService(AppDbContext context, IUserSesion userSesion)
        {
            _context = context;
            _userSesion = userSesion;
        }

        /// <summary>
        /// Creates a new work task.
        /// </summary>
        /// <param name="taskCreateDto">DTO with the information required to create a task.</param>
        /// <returns>Returns a GenericResponse with the newly created task.</returns>
        public async Task<GenericResponse<WorkTask>> CreateTaskAsync(TaskCreateRequestDto taskCreateDto)
        {
            var newTask = new WorkTask
            {
                Title = taskCreateDto.Name,
                Description = taskCreateDto.Description
            };

            await _context.WorkTasks!.AddAsync(newTask);
            await _context.SaveChangesAsync();

            return GenericResponse<WorkTask>.Success(newTask, "Task created successfully");
        }

        /// <summary>
        /// Retrieves all the tasks.
        /// </summary>
        /// <returns>Returns a list of all tasks.</returns>
        public async Task<GenericResponse<List<WorkTask>>> GetAllTasksAsync()
        {
            var tasks = await _context.WorkTasks!.ToListAsync();
            if (tasks.Count == 0)
            {
                return GenericResponse<List<WorkTask>>.Error("No tasks found");
            }
            return GenericResponse<List<WorkTask>>.Success(tasks, "All Tasks found");
        }

        /// <summary>
        /// Recovers all tasks assigned to the logged in user.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        public async Task<GenericResponse<List<WorkTask>>> GetAllTheTasksAssignedToMe() 
        {
            var userIdLogged = _userSesion.GetUserSesion();

            var task = await _context.WorkTasks!.Where(r=> r.AssignedToUserId == userIdLogged).ToListAsync();
            if (task == null)
            {
                return GenericResponse<List<WorkTask>>.Error("Task not found");
            }
            return GenericResponse<List<WorkTask>>.Success(task, "Task found");
        }

        /// <summary>
        /// Retrieves a specific task by ID.
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <returns>Returns the task if found, otherwise an error message.</returns>
        public async Task<GenericResponse<WorkTask>> GetTaskByIdAsync(Guid id)
        {
            var task = await _context.WorkTasks!.FindAsync(id);
            if (task == null)
            {
                return GenericResponse<WorkTask>.Error("Task not found");
            }
            return GenericResponse<WorkTask>.Success(task, "Task found");
        }

        /// <summary>
        /// Updates a task by its ID.
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <param name="taskUpdateDto">DTO containing the updated task details.</param>
        /// <returns>Returns the updated task or an error if not found.</returns>
        public async Task<GenericResponse<WorkTask>> UpdateTaskAsync(Guid id, TaskUpdateRequestDto taskUpdateDto)
        {
            var task = await _context.WorkTasks!.FindAsync(id);
            if (task == null)
            {
                return GenericResponse<WorkTask>.Error("Task not found");
            }

            task.Title = taskUpdateDto.Name;
            task.Description = taskUpdateDto.Description;

            await _context.SaveChangesAsync();

            return GenericResponse<WorkTask>.Success(task, "Task updated successfully");
        }

        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <returns>Returns a success message or an error if the task was not found.</returns>
        public async Task<GenericResponse<bool>> DeleteTaskAsync(Guid id)
        {
            var task = await _context.WorkTasks!.FindAsync(id);
            if (task == null)
            {
                return GenericResponse<bool>.Error("Task not found");
            }

            _context.WorkTasks.Remove(task);
            await _context.SaveChangesAsync();

            return GenericResponse<bool>.Success(true, "Task deleted successfully");
        }
    }
}
