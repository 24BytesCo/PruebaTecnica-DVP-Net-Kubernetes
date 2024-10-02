using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Token;
using System.Collections.Generic;
using System.Data;
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
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkTaskService> _logger;
        /// <summary>
        /// Constructor for WorkTaskService.
        /// </summary>
        /// <param name="context">Database context to be injected.</param>
        public WorkTaskService(AppDbContext context, IUserSesion userSesion, RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapper)
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

            var emailUserLogged = _userSesion.GetUserSesion();

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
        public async Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> GetAllTasksAsync(int page, int pageSize)
        {
            var totalCount = await _context.WorkTasks!.CountAsync();

            var tasks = await _context.WorkTasks!
                .Include(r => r.WorkTaskStatusNavigation)
                .Include(r => r.AssignedToUserNavigation)
                .Include(r => r.CreatedByUserNavigation)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                objAssigned.UserAssignedObj!.RoleCode = roleBd.Code;

                objAssigned.UserByCreatedObj!.RoleId = roleUserBd.RoleId;
                objAssigned.UserByCreatedObj!.RoleName = roleBd.Name;
                objAssigned.UserByCreatedObj!.RoleCode = roleBd.Code;

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

            return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Success(result, "All Tasks found", totalCount);
        }

        /// <summary>
        /// Recovers all tasks assigned to the logged in user.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        public async Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> GetAllTheTasksAssignedToMe(int page = 1, int pageSize = 6)
        {
            var emailUserLogged = _userSesion.GetUserSesion();

            var user = await _userManager.FindByEmailAsync(emailUserLogged);
            var totalCount = await _context.WorkTasks!
                .Include(r => r.CreatedByUserNavigation)
                .Where(r => r.AssignedToUserId == user!.Id)
                .CountAsync();

            var tasks = await _context.WorkTasks!
                .Include(r => r.CreatedByUserNavigation)
                .Where(r => r.AssignedToUserId == user!.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


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
                objAssigned.UserAssignedObj!.RoleCode = roleBd.Code;

                objAssigned.UserByCreatedObj!.RoleId = roleUserBd.RoleId;
                objAssigned.UserByCreatedObj!.RoleName = roleBd.Name;
                objAssigned.UserByCreatedObj!.RoleCode = roleBd.Code;

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


            return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Success(result, "Task found", totalCount);
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
        public async Task<GenericResponse<bool>> UpdateTaskAsync(Guid id, TaskUpdateRequestDto requestDto)
        {
            try
            {
                if (string.IsNullOrEmpty(requestDto.Title))
                {
                    return GenericResponse<bool>.Error($"Task title is required");
                }

                // Validate input parameters
                var validationResponse = await ValidateUpdateRequestAsync(id, new() { NewUserAssignedId = requestDto.NewUserAssignedId, NewWorkTaskStateId = requestDto.NewWorkTaskStateId });
                if (!validationResponse.IsSuccess)
                {
                    return GenericResponse<bool>.Error(validationResponse.ErrorMessage);
                }

                // Retrieve the task entity
                var task = await GetWorkTaskByIdAsync(id);
                if (task == null)
                {
                    // This should not happen due to prior existence check, but added for safety
                    return GenericResponse<bool>.Error($"No task found with ID: {id}");
                }

                // Update task properties
                task.Title = requestDto.Title;
                task.Description = task.Description;
                task.AssignedToUserId = requestDto.NewUserAssignedId;
                task.WorkTaskStatusId = requestDto.NewWorkTaskStateId;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return GenericResponse<bool>.Success(true, "Task updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency issues
                _logger.LogError(ex, "Concurrency error while updating task with ID {TaskId}.", id);
                return GenericResponse<bool>.Error("The task was modified by another process. Please reload and try again.");
            }
            catch (DbUpdateException ex)
            {
                // Handle database update errors
                _logger.LogError(ex, "Database update error while updating task with ID {TaskId}.", id);
                return GenericResponse<bool>.Error("An error occurred while updating the task in the database.");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                _logger.LogError(ex, "Unexpected error while updating task with ID {TaskId}.", id);
                return GenericResponse<bool>.Error($"An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the state and assigned user of an existing task.
        /// </summary>
        /// <param name="id">Unique identifier of the task (Guid).</param>
        /// <param name="requestDto">DTO containing the new state and assigned user information.</param>
        /// <returns>Generic response indicating success or failure of the operation.</returns>
        public async Task<GenericResponse<bool>> UpdateTaskStateAndUserAssign(Guid id, UpdateStateAndUserAssignRequestDto requestDto)
        {
            try
            {
                // Validate input parameters
                var validationResponse = await ValidateUpdateRequestAsync(id, requestDto);
                if (!validationResponse.IsSuccess)
                {
                    return GenericResponse<bool>.Error(validationResponse.ErrorMessage);
                }

                // Retrieve the task entity
                var task = await GetWorkTaskByIdAsync(id);
                if (task == null)
                {
                    // This should not happen due to prior existence check, but added for safety
                    return GenericResponse<bool>.Error($"No task found with ID: {id}");
                }

                // Update task properties
                task.AssignedToUserId = requestDto.NewUserAssignedId;
                task.WorkTaskStatusId = requestDto.NewWorkTaskStateId;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return GenericResponse<bool>.Success(true, "Task updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency issues
                _logger.LogError(ex, "Concurrency error while updating task with ID {TaskId}.", id);
                return GenericResponse<bool>.Error("The task was modified by another process. Please reload and try again.");
            }
            catch (DbUpdateException ex)
            {
                // Handle database update errors
                _logger.LogError(ex, "Database update error while updating task with ID {TaskId}.", id);
                return GenericResponse<bool>.Error("An error occurred while updating the task in the database.");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                _logger.LogError(ex, "Unexpected error while updating task with ID {TaskId}.", id);
                return GenericResponse<bool>.Error($"An unexpected error occurred: {ex.Message}");
            }
        }



        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <returns>Returns a success message or an error if the task was not found.</returns>
        public async Task<GenericResponse<bool>> DeleteTaskAsync(string id)
        {
            // Capture the email of the currently logged-in user
            var emailUserLogged = _userSesion.GetUserSesion();

            // Find the logged-in user in the database using their email
            var userLogged = await _context.Users
                .SingleOrDefaultAsync(r => r.Email == emailUserLogged);

            // Verify that the logged-in user exists
            if (userLogged == null)
            {
                return GenericResponse<bool>.Error("Authenticated user not found.");
            }

            // Get the user's role from the UserRoles table
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(r => r.UserId == userLogged.Id);

            // Verify that the user-role relationship exists
            if (userRole == null)
            {
                return GenericResponse<bool>.Error("User role not found.");
            }

            // Get the role details from the Roles table
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == userRole.RoleId);

            var user = await _userManager.FindByEmailAsync(emailUserLogged);


            var task = await _context.WorkTasks!.FindAsync(id);

            if (task == null)
            {
                return GenericResponse<bool>.Error("Task not found");
            }

            // Check if the user has the "Empleado" role and if the task is assigned to them
            if (role.Name == "Empleado" && task.AssignedToUserId != userLogged.Id)
            {
                return GenericResponse<bool>.Error("Forbidden. You do not have permission to access this resource.");
            }

            _context.WorkTasks.Remove(task);
            await _context.SaveChangesAsync();

            return GenericResponse<bool>.Success(true, "Task deleted successfully");
        }

        /// <summary>
        /// Updates the state of a task by an employee.
        /// </summary>
        /// <param name="idTask">Unique identifier of the task.</param>
        /// <param name="requestDto">DTO containing the new task state.</param>
        /// <returns>A generic response indicating the success or failure of the operation.</returns>
        public async Task<GenericResponse<bool>> UpdateTaskStateByEmployeeAsync(Guid idTask, UpdateTaskStateByEmployeeRequestDto requestDto)
        {
            try
            {
                // Validate that the request DTO is not null
                if (requestDto == null)
                {
                    return GenericResponse<bool>.Error("The request data cannot be null.");
                }

                // Capture the email of the currently logged-in user
                var emailUserLogged = _userSesion.GetUserSesion();

                // Find the logged-in user in the database using their email
                var userLogged = await _context.Users
                    .SingleOrDefaultAsync(r => r.Email == emailUserLogged);

                // Verify that the logged-in user exists
                if (userLogged == null)
                {
                    return GenericResponse<bool>.Error("Authenticated user not found.");
                }

                // Get the user's role from the UserRoles table
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(r => r.UserId == userLogged.Id);

                // Verify that the user-role relationship exists
                if (userRole == null)
                {
                    return GenericResponse<bool>.Error("User role not found.");
                }

                // Get the role details from the Roles table
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == userRole.RoleId);

                // Verify that the role exists
                if (role == null)
                {
                    return GenericResponse<bool>.Error("The specified role does not exist.");
                }

                // Find the task to be updated using its ID
                var existWorkTask = await _context.WorkTasks!
                    .FirstOrDefaultAsync(r => r.TaskId == idTask.ToString());

                // Check if the task exists
                if (existWorkTask == null)
                {
                    return GenericResponse<bool>.Error($"The task to update does not exist, TaskId: {idTask}");
                }

                // Check if the user has the "Empleado" role and if the task is assigned to them
                if (role.Name == "Empleado" && existWorkTask.AssignedToUserId != userLogged.Id)
                {
                    return GenericResponse<bool>.Error("Forbidden. You do not have permission to access this resource.");
                }

                // Verify if the new task state exists in the database
                var existWorkTaskState = await _context.WorkTaskStatuses!
                    .FirstOrDefaultAsync(r => r.WorkTaskStatusId == requestDto.NewWorkTaskStateId);

                // Check if the task state exists
                if (existWorkTaskState == null)
                {
                    return GenericResponse<bool>.Error($"The new state you want to update does not exist, TaskStateId: {requestDto.NewWorkTaskStateId}");
                }

                // Update the task's state with the new provided state
                existWorkTask.WorkTaskStatusId = existWorkTaskState.WorkTaskStatusId;

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return a successful response indicating the task state was updated
                return GenericResponse<bool>.Success(true, "Task status updated successfully.");
            }
            catch (Exception ex)
            {
                // In case of an exception, return an error response with the exception message
                return GenericResponse<bool>.Error($"An error occurred while updating the status of your task: {ex.Message}");
            }
        }


        /// <summary>
        /// Recovers all tasks found.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        public async Task<GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>> SearchTasksDynamic(string query, int pageNumber = 1, int pageSize = 6)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error($"Search query cannot be empty");
                }

                // Capture the email of the currently logged-in user
                var emailUserLogged = _userSesion.GetUserSesion();

                // Find the logged-in user in the database using their email
                var userLogged = await _context.Users
                    .SingleOrDefaultAsync(r => r.Email == emailUserLogged);

                // Verify that the logged-in user exists
                if (userLogged == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("Authenticated user not found.");
                }

                // Get the user's role from the UserRoles table
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(r => r.UserId == userLogged.Id);

                // Verify that the user-role relationship exists
                if (userRole == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("User role not found.");
                }

                // Get the role details from the Roles table
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == userRole.RoleId);

                // Verify that the role exists
                if (role == null)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error("The specified role does not exist.");
                }

                var totalCount = 0;
                List<WorkTask> tasks = new();
                if (role.Code == "EMP")
                {
                    totalCount = await _context.WorkTasks!
                    .Include(r => r.WorkTaskStatusNavigation)
                    .Include(r => r.AssignedToUserNavigation)
                    .Include(r => r.CreatedByUserNavigation)
                    .Where(r => r.AssignedToUserId == userLogged.Id)
                    .Where(t => t.Title!.Contains(query) ||
                                t.Description!.Contains(query) ||
                                (t.AssignedToUserNavigation!.FirstName + " " + t.AssignedToUserNavigation.LastName).Contains(query))
                    .CountAsync();

                    tasks = await _context.WorkTasks!
                    .Include(r => r.WorkTaskStatusNavigation)
                    .Include(r => r.AssignedToUserNavigation)
                    .Include(r => r.CreatedByUserNavigation)
                    .Where(r => r.AssignedToUserId == userLogged.Id)
                    .Where(t => t.Title!.Contains(query) ||
                                t.Description!.Contains(query) ||
                                (t.AssignedToUserNavigation!.FirstName + " " + t.AssignedToUserNavigation.LastName).Contains(query))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                }
                else
                {
                    totalCount = await _context.WorkTasks!
                    .Include(r => r.WorkTaskStatusNavigation)
                    .Include(r => r.AssignedToUserNavigation)
                    .Include(r => r.CreatedByUserNavigation)
                    .Where(t => t.Title!.Contains(query) ||
                                t.Description!.Contains(query) ||
                                (t.AssignedToUserNavigation!.FirstName + " " + t.AssignedToUserNavigation.LastName).Contains(query))
                    .CountAsync();

                    tasks = await _context.WorkTasks!
                    .Include(r => r.WorkTaskStatusNavigation)
                    .Include(r => r.AssignedToUserNavigation)
                    .Include(r => r.CreatedByUserNavigation)
                    .Where(t => t.Title!.Contains(query) ||
                                t.Description!.Contains(query) ||
                                (t.AssignedToUserNavigation!.FirstName + " " + t.AssignedToUserNavigation.LastName).Contains(query))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                }

                if (tasks.Count == 0)
                {
                    return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Success(null, $"No tasks found");
                }

                var taskDto = _mapper.Map<List<GetAllTheTasksAssignedToMeResponseDto>>(tasks);
                return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Success(taskDto, "Tasks found.", totalCount);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error($"An error occurred while found task {ex.Message}");

            }
        }





        /// <summary>
        /// Validates the input parameters for updating a task.
        /// </summary>
        /// <param name="id">Unique identifier of the task.</param>
        /// <param name="requestDto">DTO containing the new state and assigned user information.</param>
        /// <returns>A tuple indicating whether validation was successful and an error message if applicable.</returns>
        private async Task<(bool IsSuccess, string ErrorMessage)> ValidateUpdateRequestAsync(Guid id, UpdateStateAndUserAssignRequestDto requestDto)
        {
            // Check if the task ID is valid
            if (id == Guid.Empty)
            {
                return (false, "The provided task ID is invalid.");
            }

            // Check if the request DTO is null
            if (requestDto == null)
            {
                return (false, "The request data cannot be null.");
            }

            // Check if the new assigned user ID is provided
            if (string.IsNullOrWhiteSpace(requestDto.NewUserAssignedId))
            {
                return (false, "The new assigned user ID cannot be empty.");
            }

            // Check if the new task state ID is provided
            if (string.IsNullOrWhiteSpace(requestDto.NewWorkTaskStateId))
            {
                return (false, "The new task state ID cannot be empty.");
            }

            // Verify that the task exists
            var taskExists = await _context.WorkTasks!.AnyAsync(w => w.TaskId == id.ToString());
            if (!taskExists)
            {
                return (false, $"No task found with ID: {id}");
            }

            // Verify that the user exists
            var userExists = await _context.Users!.AnyAsync(u => u.Id == requestDto.NewUserAssignedId);
            if (!userExists)
            {
                return (false, $"The user with ID {requestDto.NewUserAssignedId} does not exist.");
            }

            // Verify that the task state exists
            var stateExists = await _context.WorkTaskStatuses!.AnyAsync(s => s.WorkTaskStatusId == requestDto.NewWorkTaskStateId);
            if (!stateExists)
            {
                return (false, $"The task state with ID {requestDto.NewWorkTaskStateId} does not exist.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Retrieves a task entity by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the task.</param>
        /// <returns>The task entity if found; otherwise, null.</returns>
        private async Task<WorkTask?> GetWorkTaskByIdAsync(Guid id)
        {
            string taskIdString = id.ToString();
            return await _context.WorkTasks!.FirstOrDefaultAsync(w => w.TaskId == taskIdString);
        }

    }


}
