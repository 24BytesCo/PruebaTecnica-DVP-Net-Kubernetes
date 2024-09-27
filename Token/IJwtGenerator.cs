
using PruebaTecnica_DVP_Net_Kubernetes.Models;

namespace PruebaTecnica_DVP_Net_Kubernetes.Token
{
    public interface IJwtGenerator
    {
        string GenerateJwtToken(User user);
    }
}