using PruebaTecnica_DVP_Net_Kubernetes.Data.GenericRepository;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Models;

namespace PruebaTecnica_DVP_Net_Kubernetes.Data.UserRepository
{
    /// <summary>
    /// Interface for user-related operations, including authentication and session management.
    /// Extends from the generic repository to include CRUD operations.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Logs in a user with the provided credentials.
        /// </summary>
        /// <param name="email">The email of the user trying to log in.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A task representing the login operation. The task result contains a UserResponseDto if login is successful, or null otherwise.</returns>
        Task<GenericResponse<UserResponseDto>> LoginAsync(string email, string password);

        /// <summary>
        /// Logs out the currently logged in user.
        /// </summary>
        /// <returns>A task representing the logout operation. The task result is true if logout was successful.</returns>
        Task<GenericResponse<bool>> LogoutAsync();

        /// <summary>
        /// Retrieves the currently logged in user.
        /// </summary>
        /// <returns>A task representing the operation. The task result contains a UserResponseDto representing the currently logged-in user, or null if no user is logged in.</returns>
        Task<GenericResponse<UserResponseDto?>> GetLoggedUserAsync();

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="userRegisterDto">The registration details including email, password, and other user information.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains a UserResponseDto if registration is successful.</returns>
        Task<GenericResponse<UserResponseDto>> RegisterUserAsync(UserRegisterDto userRegisterDto);
    }
}
