using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.User;
using PruebaTecnica_DVP_Net_Kubernetes.Filters;
using PruebaTecnica_DVP_Net_Kubernetes.Services.Roles;
using PruebaTecnica_DVP_Net_Kubernetes.Services.UserService;
using System;

namespace PruebaTecnica_DVP_Net_Kubernetes.Controllers
{
    [ServiceFilter(typeof(EncryptResponseFilter))]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRoles();
            if (result == null)
                return Unauthorized();

            return Ok(result);
        }
    }
}
