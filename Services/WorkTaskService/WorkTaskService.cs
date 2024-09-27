using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Token;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for WorkTaskService.
        /// </summary>
        /// <param name="context">Database context to be injected.</param>
        public WorkTaskService(AppDbContext context, IUserSesion userSesion, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userSesion = userSesion;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new work task.
        /// </summary>
        /// <param name="taskCreateDto">DTO with the information required to create a task.</param>
        /// <returns>Returns a GenericResponse with the newly created task.</returns>
        public async Task<GenericResponse<NewCreateWorkTaskDto>> CreateTaskAsync(TaskCreateRequestDto taskCreateDto)
        {
            //Searching in bd for the id of the Pending Status
            var statePending = await _context.WorkTaskStatuses!.SingleOrDefaultAsync(r => r.Code == "PEN");

            if (statePending == null)
            {
                return GenericResponse<NewCreateWorkTaskDto>.Error("Status Task Pending not found");
            }

            var emailUserLogged =  _userSesion.GetUserSesion();

            var user = await _userManager.FindByEmailAsync(emailUserLogged);

            var assignedUser = await _userManager.FindByIdAsync(taskCreateDto.UserAssignedId);
            if (assignedUser == null)
            {
                return GenericResponse<NewCreateWorkTaskDto>.Error("Assigned user not found");
            }

            var newTask = new WorkTask
            {
                TaskId = Guid.NewGuid().ToString(),
                Title = taskCreateDto.Name,
                Description = taskCreateDto.Description,
                AssignedToUserId = assignedUser.Id, // Aseguramos que sea el ID del usuario asignado
                WorkTaskStatusId = statePending.WorkTaskStatusId,
                CreatedByUserId = user.Id
            };

            await _context.WorkTasks!.AddAsync(newTask);
            await _context.SaveChangesAsync();
            var newTaskDto = _mapper.Map<NewCreateWorkTaskDto>(newTask);

            return GenericResponse<NewCreateWorkTaskDto>.Success(newTaskDto, "Task created successfully");
        }

        /// <summary>
        /// Retrieves all the tasks.
        /// </summary>
        /// <returns>Returns a list of all tasks.</returns>
        public async Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> GetAllTasksAsync()
        {
            var tasks = await _context.WorkTasks!
                .Include(r=> r.WorkTaskStatusNavigation)
                .Include(r=> r.AssignedToUserNavigation)
                .Include(r=> r.CreatedByUserNavigation)
                .ToListAsync();
            if (tasks.Count == 0)
            {
                return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("No tasks found");
            }
            var taskDto = _mapper.Map<List<GetAllTheTasksAssignedToMeResponseDto>>(tasks);

            List<GetAllTheTasksAssignedToMeResponseDto> result = new();

            foreach (var taskItem in taskDto)
            {
                GetAllTheTasksAssignedToMeResponseDto objAssigned = new();
                objAssigned = taskItem;

                objAssigned.UserAssignedObj!.NameCompleted = taskItem.UserAssignedObj!.FirstName + " " + taskItem.UserAssignedObj!.LastName;

                objAssigned.UserByCreatedObj!.NameCompleted = taskItem.UserByCreatedObj!.FirstName + " " + taskItem.UserByCreatedObj!.LastName;

                var roleUserBd = await _context.UserRoles.SingleOrDefaultAsync(r => r.UserId == taskItem.UserAssignedObj!.UserId);

                if (roleUserBd == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("The assigned user has no roles assigned, UserId: " + taskItem.UserAssignedObj!.UserId);
                }

                var roleBd = await _context.Roles.SingleOrDefaultAsync(r => r.Id == roleUserBd.RoleId);

                if (roleBd == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("The assigned role does not exist, RoleId: " + roleUserBd.RoleId);

                }

                objAssigned.UserAssignedObj!.RoleId = roleUserBd.RoleId;
                objAssigned.UserAssignedObj!.RoleName = roleBd.Name;

                objAssigned.UserByCreatedObj!.RoleId = roleUserBd.RoleId;
                objAssigned.UserByCreatedObj!.RoleName = roleBd.Name;

                var workTaskStateBd = await _context.WorkTasks!
                    .Include(t => t.WorkTaskStatusNavigation)
                    .FirstOrDefaultAsync(r => r.TaskId == taskItem.WorkTaskId);

                if (workTaskStateBd == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("The assigned task does not exist in the system, WorkTaskId: " + taskItem.WorkTaskId);
                }

                objAssigned.WorkTaskStateObj!.WorkTaskStateId = workTaskStateBd.WorkTaskStatusNavigation!.WorkTaskStatusId;
                objAssigned.WorkTaskStateObj!.Code = workTaskStateBd.WorkTaskStatusNavigation!.Code;
                objAssigned.WorkTaskStateObj!.Name = workTaskStateBd.WorkTaskStatusNavigation!.Name;
                objAssigned.WorkTaskStateObj!.Description = workTaskStateBd.WorkTaskStatusNavigation!.Description;

                result.Add(objAssigned);
            }

            return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Success(result, "All Tasks found");
        }

        /// <summary>
        /// Recovers all tasks assigned to the logged in user.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        public async Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> GetAllTheTasksAssignedToMe() 
        {
            var emailUserLogged = _userSesion.GetUserSesion();

            var user = await _userManager.FindByEmailAsync(emailUserLogged);
            var tasks = await _context.WorkTasks!.Where(r=> r.AssignedToUserId == user!.Id).ToListAsync();
            if (tasks == null)
            {
                return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("Task not found");
            }
            var taskDto = _mapper.Map<List<GetAllTheTasksAssignedToMeResponseDto>>(tasks);

            List<GetAllTheTasksAssignedToMeResponseDto> result = new(); 

            foreach (var taskItem in taskDto)
            {
                GetAllTheTasksAssignedToMeResponseDto objAssigned = new();
                objAssigned = taskItem;
                
                objAssigned.UserAssignedObj!.NameCompleted = taskItem.UserAssignedObj!.FirstName +" "+ taskItem.UserAssignedObj!.LastName;
                
                objAssigned.UserByCreatedObj!.NameCompleted = taskItem.UserByCreatedObj!.FirstName +" "+ taskItem.UserByCreatedObj!.LastName;
                
                var roleUserBd = await _context.UserRoles.SingleOrDefaultAsync(r => r.UserId == taskItem.UserAssignedObj!.UserId);

                if (roleUserBd == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("The assigned user has no roles assigned, UserId: " + taskItem.UserAssignedObj!.UserId);
                }

                var roleBd = await _context.Roles.SingleOrDefaultAsync(r => r.Id == roleUserBd.RoleId);

                if (roleBd == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("The assigned role does not exist, RoleId: " + roleUserBd.RoleId);

                }

                objAssigned.UserAssignedObj!.RoleId = roleUserBd.RoleId;
                objAssigned.UserAssignedObj!.RoleName = roleBd.Name;

                objAssigned.UserByCreatedObj!.RoleId = roleUserBd.RoleId;
                objAssigned.UserByCreatedObj!.RoleName = roleBd.Name;

                var workTaskStateBd = await _context.WorkTasks!
                    .Include(t => t.WorkTaskStatusNavigation)
                    .FirstOrDefaultAsync(r=> r.TaskId == taskItem.WorkTaskId);

                if (workTaskStateBd == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("The assigned task does not exist in the system, WorkTaskId: " + taskItem.WorkTaskId);
                }

                objAssigned.WorkTaskStateObj!.WorkTaskStateId = workTaskStateBd.WorkTaskStatusNavigation!.WorkTaskStatusId;
                objAssigned.WorkTaskStateObj!.Code = workTaskStateBd.WorkTaskStatusNavigation!.Code;
                objAssigned.WorkTaskStateObj!.Name = workTaskStateBd.WorkTaskStatusNavigation!.Name;
                objAssigned.WorkTaskStateObj!.Description = workTaskStateBd.WorkTaskStatusNavigation!.Description;

                result.Add(objAssigned);
            }


            return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Success(result, "Task found");
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
