

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.User;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Token;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserSesion _userSession;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;


        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IUserSesion userSession, IJwtGenerator jwtGenerator, RoleManager<Role> roleManager, IMapper mapper, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userSession = userSession;
            _jwtGenerator = jwtGenerator;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
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
                if (await _userManager.FindByEmailAsync(registerDto.Email!) != null)
                    return GenericResponse<UserResponseDto>.Error("The user already exists.");

                //Validate if the role exists
                var existRoleManager = await _roleManager.FindByIdAsync(registerDto.RoleId!);
                
                if (existRoleManager == null)
                {
                    return GenericResponse<UserResponseDto>.Error("The role to be assigned does not exist in the system");
                }

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

                //Assigning the role to the user
                await _userManager.AddToRoleAsync(newUser, existRoleManager.Name!);

                // Generate a JWT token
                var token = await _jwtGenerator.GenerateJwtToken(newUser);

                var userResponse = new UserResponseDto
                {
                    UserId = Guid.Parse(newUser.Id),
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    RoleName = existRoleManager.Name,
                    RoleId = existRoleManager.Id
                };

                return GenericResponse<UserResponseDto>.Success(userResponse, "User successfully registered.");
            }
            catch (Exception ex)
            {
                return GenericResponse<UserResponseDto>.Error($"An error occurred during registration: {ex.Message}");
            }
        }

        /// <summary>
        /// Update user in the system.
        /// </summary>
        /// <param name="updateUserRequest">User update data.</param>
        /// <returns>A GenericResponse containing a boolean indicating success or failure.</returns>
        public async Task<GenericResponse<bool>> UpdateUserRequest(UpdateUserRequest updateUserRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(updateUserRequest.RoleId))
                {
                    return GenericResponse<bool>.Error("The role to be assigned is mandatory");
                }

                if (string.IsNullOrEmpty(updateUserRequest.FirstName))
                {
                    return GenericResponse<bool>.Error("The firs name to be assigned is mandatory");
                }

                var existUser = await _context.Users.FirstOrDefaultAsync(r => r.Id == updateUserRequest.UserId);
                if (existUser == null)
                {
                    return GenericResponse<bool>.Error("The user to edit does not exist");
                }

                existUser.FirstName = updateUserRequest.FirstName;
                existUser.LastName = updateUserRequest.LastName ?? "";

                // Elimina el rol actual
                var roleUser = await _context.UserRoles.FirstOrDefaultAsync(r => r.UserId == existUser.Id);
                if (roleUser == null)
                {
                    return GenericResponse<bool>.Error("The user has no roles assigned");
                }

                _context.UserRoles.Remove(roleUser); // Eliminamos la relación actual

                var existRoleNew = await _context.Roles.FirstOrDefaultAsync(r => r.Id == updateUserRequest.RoleId);
                if (existRoleNew == null)
                {
                    return GenericResponse<bool>.Error("The role to be assigned does not exist");
                }

                // Asigna el nuevo rol
                var newUserRole = new IdentityUserRole<string>
                {
                    UserId = existUser.Id,
                    RoleId = updateUserRequest.RoleId
                };

                _context.UserRoles.Add(newUserRole); // Añadimos la nueva relación de rol

                await _context.SaveChangesAsync();

                return GenericResponse<bool>.Success(true, "User updated successfully");
            }
            catch (Exception ex)
            {
                return GenericResponse<bool>.Error($"An error occurred while editing the user: {ex.Message}");
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

                var token = await _jwtGenerator.GenerateJwtToken(user);

                // Validate if the user has any roles assigned
                var roles = await _userManager.GetRolesAsync(user);

                if (roles == null || !roles.Any())
                {
                    return GenericResponse<UserResponseDto>.Error("The user has no roles assigned.");
                }

                // Find the role by name
                var roleName = roles.FirstOrDefault();
                if (string.IsNullOrEmpty(roleName))
                {
                    return GenericResponse<UserResponseDto>.Error("The role name is invalid.");
                }

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return GenericResponse<UserResponseDto>.Error("Role not found.");
                }

                var userResponse = new UserResponseDto
                {
                    UserId = Guid.Parse(user.Id),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = role.Name,
                    RoleId = role.Id,
                    RoleCode = role.Code,
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
                var userId = _userSession.GetUserSesion();
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

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>A GenericResponse indicating the result of the logout operation.</returns>
        public async Task<GenericResponse<bool>> LogoutUserAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return GenericResponse<bool>.Success(true, "User successfully logged out.");
            }
            catch (Exception ex)
            {
                return GenericResponse<bool>.Error($"An error occurred during logout: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>A GenericResponse containing the user's details. reducer</returns>
        public async Task<GenericResponse<List<UserReducerResponseDto>>?> GetAllUsersReducer(int page = 1, int pageSize = 6)
        {
            try
            {
                // Obtener todos los usuarios en una consulta
                var totalCount = await _context.Users.CountAsync();
                var users = await _context.Users
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                List<UserReducerResponseDto> result = new();

                foreach (var user in users)
                {
                    // Obtener roles de forma secuencial para evitar problemas de conexión
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles == null || !roles.Any())
                    {
                        return GenericResponse<List<UserReducerResponseDto>>.Error("The user has no roles assigned.");
                    }

                    // Obtenemos el primer rol y validamos su existencia
                    var roleName = roles.FirstOrDefault();
                    if (string.IsNullOrEmpty(roleName))
                    {
                        return GenericResponse<List<UserReducerResponseDto>>.Error("The role name is invalid.");
                    }

                    // Buscamos el rol en la base de datos
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role == null)
                    {
                        return GenericResponse<List<UserReducerResponseDto>>.Error("Role not found.");
                    }

                    // Creamos el DTO con los datos del usuario
                    var userResponse = new UserReducerResponseDto
                    {
                        UserId = Guid.Parse(user.Id),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        RoleName = role.Name,
                        RoleId = role.Id,
                        RoleCode = role.Code,
                        Email = user.Email
                    };

                    result.Add(userResponse);
                }

                return GenericResponse<List<UserReducerResponseDto>>.Success(result, "All users found", totalCount);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<UserReducerResponseDto>>.Error($"An error occurred during consulting users: {ex.Message}");
            }
        }

        /// <summary>
        /// Recovers all users found.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        public async Task<GenericResponse<List<UserReducerResponseDto>>> SearchTasksDynamic(string query, int pageNumber = 1, int pageSize = 6) 
        {
            try
            {

                // Admin can search all users
                var totalCount = await _context.Users
                    .Where(u => u.FirstName!.Contains(query) ||
                                u.LastName!.Contains(query) ||
                                u.Email!.Contains(query))
                    .CountAsync();

                List<User> users = await _context.Users
                    .Where(u => u.FirstName!.Contains(query) ||
                                u.LastName!.Contains(query) ||
                                u.Email!.Contains(query))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                List<UserReducerResponseDto> result = new();


                foreach (var user in users)
                {
                    // Obtener roles de forma secuencial para evitar problemas de conexión
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles == null || !roles.Any())
                    {
                        return GenericResponse<List<UserReducerResponseDto>>.Error("The user has no roles assigned.");
                    }

                    // Obtenemos el primer rol y validamos su existencia
                    var roleName = roles.FirstOrDefault();
                    if (string.IsNullOrEmpty(roleName))
                    {
                        return GenericResponse<List<UserReducerResponseDto>>.Error("The role name is invalid.");
                    }

                    // Buscamos el rol en la base de datos
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role == null)
                    {
                        return GenericResponse<List<UserReducerResponseDto>>.Error("Role not found.");
                    }

                    // Creamos el DTO con los datos del usuario
                    var userResponse = new UserReducerResponseDto
                    {
                        UserId = Guid.Parse(user.Id),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        RoleName = role.Name,
                        RoleId = role.Id,
                        RoleCode = role.Code,
                        Email = user.Email
                    };

                    result.Add(userResponse);
                }
                return GenericResponse<List<UserReducerResponseDto>>.Success(result, "Users found.", totalCount);
            }
            catch (Exception ex)
            {
                return GenericResponse<List<UserReducerResponseDto>>.Error($"An error occurred while retrieving users: {ex.Message}");
            }
        }

        /// <summary>
        ///delete user
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        public async Task<GenericResponse<bool>> DeleteUserAsync(string id) 
        {
            try
            {
                // Buscar el usuario por ID
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    return GenericResponse<bool>.Error("The user does not exist.");
                }

                var tasks = await _context.WorkTasks!.Where(t => t.AssignedToUserId == user.Id).ToListAsync();

                _context.WorkTasks!.RemoveRange(tasks);


                await _context.SaveChangesAsync();

                // Eliminar el usuario
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return GenericResponse<bool>.Success(true, "User deleted successfully.");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return GenericResponse<bool>.Error($"Failed to delete the user. Errors: {errors}");
                }
            }
            catch (Exception ex)
            {
                return GenericResponse<bool>.Error($"An error occurred while deleting the user: {ex.Message}");
            }
        }

    }
}

