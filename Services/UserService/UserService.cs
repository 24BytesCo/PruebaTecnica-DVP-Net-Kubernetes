

using Microsoft.AspNetCore.Identity;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Token;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserSesion _userSesion;
        private readonly IJwtGenerator _jwtGenerator;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IUserSesion userSesion, IJwtGenerator jwtGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userSesion = userSesion;
            _jwtGenerator = jwtGenerator;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="registerDto">User registration data.</param>
        /// <returns>A GenericResponse containing UserResponseDto with user details and a JWT token.</returns>
        public async Task<GenericResponse<UserResponseDto>> RegisterUserAsync(UserRegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                    return GenericResponse<UserResponseDto>.Error("The user already exists.");

                var newUser = new User
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName
                };

                var result = await _userManager.CreateAsync(newUser, registerDto.Password);

                if (!result.Succeeded)
                    return GenericResponse<UserResponseDto>.Error("Error creating the user: " + string.Join(", ", result.Errors));

                // Generate a JWT token
                var token = _jwtGenerator.GenerateJwtToken(newUser);

                var userResponse = new UserResponseDto
                {
                    UserId = Guid.Parse(newUser.Id),
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Token = token
                };

                return GenericResponse<UserResponseDto>.Success(userResponse, "User successfully registered.");
            }
            catch (Exception ex)
            {
                return GenericResponse<UserResponseDto>.Error($"An error occurred during registration: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs in a user with email and password.
        /// </summary>
        /// <param name="loginDto">User login data.</param>
        /// <returns>A GenericResponse containing UserResponseDto with user details and a JWT token.</returns>
        public async Task<GenericResponse<UserResponseDto>> LoginUserAsync(UserLoginRequestDto loginDto)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, false);

                if (!result.Succeeded)
                    return GenericResponse<UserResponseDto>.Error("Invalid login attempt.");

                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                    return GenericResponse<UserResponseDto>.Error("User not found.");

                var token = _jwtGenerator.GenerateJwtToken(user);

                var userResponse = new UserResponseDto
                {
                    UserId = Guid.Parse(user.Id),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = token
                };

                return GenericResponse<UserResponseDto>.Success(userResponse, "Login successful.");
            }
            catch (Exception ex)
            {
                return GenericResponse<UserResponseDto>.Error($"An error occurred during login: {ex.Message}");
            }
        }

        /// <summary>
        /// Get the currently logged-in user.
        /// </summary>
        /// <returns>A GenericResponse containing the user's information.</returns>
        public async Task<GenericResponse<UserResponseDto?>> GetLoggedUserAsync()
        {
            try
            {
                var userId = _userSesion.GetUserSesion();
                if (userId == null)
                    return GenericResponse<UserResponseDto?>.Error("No user is currently logged in.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return GenericResponse<UserResponseDto?>.Error("User not found.");

                var userResponse = new UserResponseDto
                {
                    UserId = Guid.Parse(user.Id),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                return GenericResponse<UserResponseDto?>.Success(userResponse, "User retrieved successfully.");
            }
            catch (Exception ex)
            {
                return GenericResponse<UserResponseDto?>.Error($"An error occurred while retrieving the user: {ex.Message}");
            }
        }
    }
}

