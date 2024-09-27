using Microsoft.AspNetCore.Identity;
using PruebaTecnica_DVP_Net_Kubernetes.Data.GenericRepository;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Token;

namespace PruebaTecnica_DVP_Net_Kubernetes.Data.UserRepository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserSesion _userSesion;
        private readonly IJwtGenerator _jwtGenerator;

        public UserRepository(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IUserSesion userSesion, IJwtGenerator jwtGenerator)
            : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userSesion = userSesion;
            _jwtGenerator = jwtGenerator;
        }

        /// <summary>
        /// Logs in a user with the provided credentials.
        /// </summary>
        public async Task<GenericResponse<UserResponseDto>> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        return GenericResponse<UserResponseDto>.Success(TransformingUserIntoUserResponseDto(user));
                    }
                }

                throw new ApplicationException("Invalid login attempt.");
            }
            catch (Exception ex)
            {
                return GenericResponse<UserResponseDto>.Error($"An error occurred during login: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs out the currently logged in user.
        /// </summary>
        public async Task<GenericResponse<bool>> LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return GenericResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return GenericResponse<bool>.Error($"An error occurred while logging out: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the currently logged in user.
        /// </summary>
        public async Task<GenericResponse<UserResponseDto?>> GetLoggedUserAsync()
        {
            try
            {
                var userId = _userSesion.GetUserSesion();
                if (userId != null)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        return GenericResponse<UserResponseDto?>.Success(TransformingUserIntoUserResponseDto(user));
                    }
                }

            return GenericResponse<UserResponseDto?>.Success(null);

            }
            catch (Exception ex)
            {
                return GenericResponse<UserResponseDto?>.Error($"An error occurred while retrieving the logged-in user: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="userRegisterDto">The registration details including email, password, and other user information.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a UserResponseDto if registration is successful.</returns>
        public async Task<GenericResponse<UserResponseDto>> RegisterUserAsync(UserRegisterDto userRegisterDto)
        {
            try
            {
                var newUser = new User
                {
                    UserName = userRegisterDto.Email,
                    Email = userRegisterDto.Email,
                    FirstName = userRegisterDto.FirstName,
                    LastName = userRegisterDto.LastName
                };

                // Register the user with the specified password
                var result = await _userManager.CreateAsync(newUser, userRegisterDto.Password!);

                if (result.Succeeded)
                {

                    // Generate JWT token for the newly registered user
                    var token = _jwtGenerator.GenerateJwtToken(newUser);

                    UserResponseDto newUserDTo = new ()
                    {
                        UserId = Guid.Parse(newUser.Id),
                        Email = newUser.Email,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Token = token
                    };

                    return GenericResponse<UserResponseDto>.Success(newUserDTo);
                }

                // If the registration fails, collect errors and throw an exception
                throw new ApplicationException($"User registration failed: {string.Join(", ", result.Errors)}");
            }
            catch (Exception ex)
            {
                return GenericResponse<UserResponseDto>.Error($"An error occurred during user registration: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method to transform a User entity into a UserResponseDto.
        /// </summary>
        private UserResponseDto TransformingUserIntoUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                UserId = Guid.Parse(user.Id),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = _jwtGenerator.GenerateJwtToken(user)
            };
        }
    }
}
