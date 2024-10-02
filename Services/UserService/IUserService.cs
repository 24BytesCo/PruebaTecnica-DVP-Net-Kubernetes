using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.User;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.UserService
{
    /// <summary>
    /// Interface defining user-related services.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="userRegisterDto">User registration data.</param>
        /// <returns>A GenericResponse containing UserResponseDto with user details and a JWT token.</returns>
        Task<GenericResponse<UserResponseDto>> RegisterUserAsync(UserRegisterDto userRegisterDto);

        /// <summary>
        /// Update user in the system.
        /// </summary>
        /// <param name="updateUserRequest">User update data.</param>
        /// <returns>A GenericResponse containing UserResponseDto with user details and a JWT token.</returns>
        Task<GenericResponse<bool>> UpdateUserRequest(UpdateUserRequest updateUserRequest);

        /// <summary>
        /// Logs in a user with email and password.
        /// </summary>
        /// <param name="userLoginRequestDto">User login data.</param>
        /// <returns>A GenericResponse containing UserResponseDto with user details and a JWT token.</returns>
        Task<GenericResponse<UserResponseDto>> LoginUserAsync(UserLoginRequestDto userLoginRequestDto);

        /// <summary>
        /// Retrieves the currently logged-in user.
        /// </summary>
        /// <returns>A GenericResponse containing the logged-in user's details.</returns>
        Task<GenericResponse<UserResponseDto?>> GetLoggedUserAsync();
        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>A GenericResponse indicating the result of the logout operation.</returns>
        Task<GenericResponse<bool>> LogoutUserAsync();

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>A GenericResponse containing the user's details. reducer</returns>
        Task<GenericResponse<List<UserReducerResponseDto>>?> GetAllUsersReducer(int page = 1, int pageSize = 6);

        /// <summary>
        /// Recovers all users found.
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        Task<GenericResponse<List<UserReducerResponseDto>>> SearchTasksDynamic(string query, int pageNumber = 1, int pageSize = 6);

        /// <summary>
        ///delete user
        /// </summary>
        /// <returns>Returns a list of all tasks assigned to the logged in user.</returns>
        Task<GenericResponse<bool>> DeleteUserAsync(string id);
    }
}