using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.User;
using PruebaTecnica_DVP_Net_Kubernetes.Filters;
using PruebaTecnica_DVP_Net_Kubernetes.Services.UserService;
using System;

namespace PruebaTecnica_DVP_Net_Kubernetes.Controllers
{
    [ServiceFilter(typeof(EncryptResponseFilter))]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Policy = "RequireAdminOrSupervisorRole")]
        [HttpGet("GetAllUsersReducer")]
        public async Task<IActionResult> GetAllUsersReducer(int page = 1, int pageSize = 6)
        {
            var result = await _userService.GetAllUsersReducer(page, pageSize);
            if (result == null)
                return Unauthorized();

            return Ok(result);
        }
    }
}
