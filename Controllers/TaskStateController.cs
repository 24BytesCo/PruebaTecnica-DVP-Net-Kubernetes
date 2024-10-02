using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.User;
using PruebaTecnica_DVP_Net_Kubernetes.Filters;
using PruebaTecnica_DVP_Net_Kubernetes.Services.TaskState;
using PruebaTecnica_DVP_Net_Kubernetes.Services.UserService;
using System;

namespace PruebaTecnica_DVP_Net_Kubernetes.Controllers
{
    [ServiceFilter(typeof(EncryptResponseFilter))]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskStateController : ControllerBase
    {
        private readonly ITaskStateService _taskStateService;

        public TaskStateController(ITaskStateService taskStateService)
        {
            _taskStateService = taskStateService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTaskStates()
        {
            var result = await _taskStateService.GetAllTaskStates();
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
