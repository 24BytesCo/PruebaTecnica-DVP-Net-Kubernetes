using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Services.UserService;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Controllers
{
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

        // Permite acceso anónimo (sin requerir token)
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequestDto userLoginRequestDto)
        {
            var result = await _userService.LoginUserAsync(userLoginRequestDto);
            return Ok(result);
        }

        // Endpoint que requiere autenticación (requiere un token válido)
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _userService.LogoutUserAsync();
            if (!result.IsSuccessful)
                return BadRequest(result.Message);

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
    }
}
