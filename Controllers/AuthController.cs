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
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // Endpoint que requiere autorización basada en el rol Admin
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            var result = await _userService.RegisterUserAsync(userRegisterDto);
            return Ok(result);
        }

        // Endpoint que requiere autorización basada en el rol Admin
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UpdateUserRequest updateUserRequest)
        {
            var result = await _userService.UpdateUserRequest(updateUserRequest);
            return Ok(result);
        }


        // Permite acceso anónimo (sin requerir token)
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequestDto userLoginRequestDto)
        {
            var result = await _userService.LoginUserAsync(userLoginRequestDto);

            if (result.IsSuccessful)
            {
                // Retorna la respuesta genérica de éxito con los datos
                return Ok(result);
            }

            // Si falla, retorna una respuesta genérica de error
            return BadRequest(result);
        }

        // Endpoint que requiere autenticación (requiere un token válido)
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _userService.LogoutUserAsync();
            if (!result.IsSuccessful)
                return BadRequest(GenericResponse<bool>.Error(result.Message));

            return Ok(result);
        }

        // Endpoint que requiere autenticación (requiere un token válido)
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetLoggedUser()
        {
            var result = await _userService.GetLoggedUserAsync();
            if (result == null)
                return Unauthorized();

            return Ok(result);
        }

        [HttpGet("SearchUsersDynamic")]
        public async Task<IActionResult> SearchUsersDynamic(string query, int page = 1, int pageSize = 6)
        {
            var result = await _userService.SearchTasksDynamic(query, page, pageSize);
            if (!result.IsSuccessful)
            {
                return NotFound(result.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.IsSuccessful)
            {
                return NotFound(result.Message);
            }
            return Ok(result);
        }
    }
}
