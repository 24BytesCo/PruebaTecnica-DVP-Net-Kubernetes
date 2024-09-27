
using PruebaTecnica_DVP_Net_Kubernetes.Models;

namespace PruebaTecnica_DVP_Net_Kubernetes.Token
{
    public interface IJwtGenerator
    {
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is being generated.</param>
        /// <returns>A string representation of the generated JWT token.</returns>
        Task<string> GenerateJwtToken(User user);
    }
}