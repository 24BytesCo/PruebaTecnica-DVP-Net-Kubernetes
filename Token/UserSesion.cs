using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PruebaTecnica_DVP_Net_Kubernetes.Token
{
    /// <summary>
    /// Class responsible for handling user session information.
    /// Provides methods to retrieve session data such as the user's ID.
    /// </summary>
    public class UserSesion : IUserSesion
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Constructor that injects an IHttpContextAccessor to access the HTTP context.
        /// </summary>
        /// <param name="httpContextAccessor">The IHttpContextAccessor instance used to access the current HTTP context.</param>
        public UserSesion(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves the current user's session ID (NameIdentifier claim).
        /// </summary>
        /// <returns>The ID of the user retrieved from the NameIdentifier claim, or null if not found.</returns>
        public string GetUserSesion()
        {
            var userId = _httpContextAccessor.HttpContext!.User.Claims?
            .FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId!;
        }
    }
}
