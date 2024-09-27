
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Services.WorkTaskService;
using System.Threading.Tasks;

    namespace PruebaTecnica_DVP_Net_Kubernetes.Controllers
    {
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
            [HttpPost("create")]
            public async Task<IActionResult> CreateTask([FromBody] TaskCreateRequestDto requestDto)
            {
                var result = await _taskService.CreateTaskAsync(requestDto);
                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }

        // Obtener todas las tareas
        [Authorize(Policy = "RequireAdminRole")]
        [Authorize(Policy = "RequireSupervisorRole")]
        [HttpGet("all")]
            public async Task<IActionResult> GetAllTasks()
            {
                var result = await _taskService.GetAllTasksAsync();
                if (!result.IsSuccessful)
                {
                    return NotFound(result.Message);
                }
                return Ok(result);
            }

        [HttpGet("GetAllAssignedToMe")]
        public async Task<IActionResult> GetAllAssignedToMe()
        {
            var result = await _taskService.GetAllTasksAsync();
            if (!result.IsSuccessful)
            {
                return NotFound(result.Message);
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
                    return NotFound(result.Message);
                }
                return Ok(result);
            }

        // Actualizar una tarea
        [Authorize(Policy = "RequireAdminRole")]
        [Authorize(Policy = "RequireSupervisorRole")]
        [HttpPut("{id}")]
            public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskUpdateRequestDto requestDto)
            {
                var result = await _taskService.UpdateTaskAsync(id, requestDto);
                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }

            // Eliminar una tarea
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteTask(Guid id)
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (!result.IsSuccessful)
                {
                    return NotFound(result.Message);
                }
                return Ok(result);
            }
        }
    }

