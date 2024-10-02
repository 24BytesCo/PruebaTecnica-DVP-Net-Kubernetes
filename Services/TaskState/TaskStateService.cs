using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.taskStates;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services.TaskState
{
    public class TaskStateService : ITaskStateService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TaskStateService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GenericResponse<List<TaskStatesResponseDto>>> GetAllTaskStates()
        {
            try
            {
                var allTaskStates = await _context.WorkTaskStatuses!.ToListAsync();


                var result = _mapper.Map<List<TaskStatesResponseDto>>(allTaskStates);

                return GenericResponse<List<TaskStatesResponseDto>>.Success(result, "All task states found");

            }
            catch (Exception ex)
            {
                return GenericResponse<List<TaskStatesResponseDto>>.Error($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
