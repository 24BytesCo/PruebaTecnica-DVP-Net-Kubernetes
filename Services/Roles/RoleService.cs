using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.Role;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RoleService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GenericResponse< List<AllRolesResponseDto>>> GetAllRoles()
        {
            try
            {
                var allRoles = await _context.Roles.ToListAsync();

                var response = _mapper.Map<List<AllRolesResponseDto>>(allRoles);

                return GenericResponse<List<AllRolesResponseDto>>.Success(response, "All roles fount"); 
            }
            catch (Exception ex)
            {
                return GenericResponse<List<AllRolesResponseDto>>.Error($"An error occurred while querying the roles: {ex.Message}");
            }
        }
    }
}
