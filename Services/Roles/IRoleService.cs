using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.Role;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.Roles
{
    public interface IRoleService
    {
        Task<GenericResponse< List<AllRolesResponseDto>>> GetAllRoles();
    }
}
