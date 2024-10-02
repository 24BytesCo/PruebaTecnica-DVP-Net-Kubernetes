
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Filters;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Services.WorkTaskService;

namespace PruebaTecnica_DVP_Net_Kubernetes.Controllers
{
    [ServiceFilter(typeof(EncryptResponseFilter))]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IWorkTaskService _taskService;

        public TaskController(IWorkTaskService taskService)
        {
            _taskService = taskService;
        }

        // Crear una nueva tarea
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateRequestDto requestDto)
        {
            var result = await _taskService.CreateTaskAsync(requestDto);
            if (!result.IsSuccessful)
            {
                return BadRequest(GenericResponse<NewCreateWorkTaskDto>.Error(result.Message));
            }
            return Ok(result);
        }

        // Obtener todas las tareas
        [Authorize(Roles = "Administrador, Supervisor")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTasks(int page = 1, int pageSize = 6)
        {
            var result = await _taskService.GetAllTasksAsync(page, pageSize);
            if (!result.IsSuccessful)
            {
                return NotFound(GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error(result.Message));
            }
            return Ok(result);
        }

        [HttpGet("GetAllAssignedToMe")]
        public async Task<IActionResult> GetAllAssignedToMe(int page = 1, int pageSize = 6)
        {
            var result = await _taskService.GetAllTheTasksAssignedToMe(page, pageSize);
            if (!result.IsSuccessful)
            {
                return NotFound(GenericResponse<List<GetAllTheTasksAssignedToMeResponseDto>>.Error(result.Message));
            }
            return Ok(result);
        }

        // Obtener una tarea específica por su ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);
            if (!result.IsSuccessful)
            {
                return BadRequest(GenericResponse<WorkTask>.Error(result.Message));
            }
            return Ok(result);
        }

        // Actualizar una tarea
        [Authorize(Roles = "Administrador, Supervisor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskUpdateRequestDto requestDto)
        {
            var result = await _taskService.UpdateTaskAsync(id, requestDto);
            if (!result.IsSuccessful)
            {
                return BadRequest(GenericResponse<bool>.Error(result.Message));
            }
            return Ok(result);
        }

        // Actualizar una tarea
        /// <summary>
        /// Updates the state and assigned user of an existing task.
        /// </summary>
        /// <param name="id">Unique identifier of the task (Guid).</param>
        /// <param name="requestDto">DTO containing the new state and assigned user information.</param>
        /// <returns>HTTP response indicating the result of the operation.</returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("{id}/UpdateTaskStateAndUserAssign")]
        public async Task<IActionResult> UpdateTaskStateAndUserAssign(Guid id, [FromBody] UpdateStateAndUserAssignRequestDto requestDto)
        {
            // Validate the input DTO
            if (requestDto == null)
            {
                return BadRequest(GenericResponse<bool>.Error($"Request data cannot be null."));

            }

            // Call the service method to update the task
            var result = await _taskService.UpdateTaskStateAndUserAssign(id, requestDto);

            // Check if the operation was successful
            if (!result.IsSuccessful)
            {
                return BadRequest(GenericResponse<bool>.Error(result.Message));
            }

            // Return a successful response with the result
            return Ok(result);
        }

        // Eliminar una tarea
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result.IsSuccessful)
            {
                return NotFound(result.Message);
            }
            return Ok(result);
        }

        [Authorize(Policy = "RequireAdminOrSupervisorOrEmployedRole")]
        [HttpPut("{id}/UpdateTaskState")]
        public async Task<IActionResult> UpdateTaskState(Guid id, [FromBody] UpdateTaskStateByEmployeeRequestDto requestDto)
        {
            var result = await _taskService.UpdateTaskStateByEmployeeAsync(id, requestDto);
            if (!result.IsSuccessful)
            {
                return BadRequest(GenericResponse<bool>.Error( result.Message));
            }
            return Ok(result);
        }

        [HttpGet("SearchTasksDynamic")]
        public async Task<IActionResult> SearchTasksDynamic(string query, int page = 1, int pageSize = 6)
        {
            var result = await _taskService.SearchTasksDynamic(query, page, pageSize);
            if (!result.IsSuccessful)
            {
                return NotFound(result.Message);
            }
            return Ok(result);
        }
    }
}

