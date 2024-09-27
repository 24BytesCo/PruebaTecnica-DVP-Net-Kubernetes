using AutoMapper;
using PruebaTecnica_DVP_Net_Kubernetes.Dtos.WorkTask;
using PruebaTecnica_DVP_Net_Kubernetes.Models;

namespace PruebaTecnica_DVP_Net_Kubernetes.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo con personalización de propiedades
            CreateMap<GetAllTheTasksAssignedToMeResponseDto, WorkTask>()
                .ForMember(destination => destination.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(destination => destination.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(destination => destination.TaskId, opt => opt.MapFrom(src => src.WorkTaskId))

                // Mapeo de TaskStatusNavigation!
                .ForPath(destination => destination.WorkTaskStatusNavigation!.WorkTaskStatusId, opt => opt.MapFrom(src => src.WorkTaskStateObj!.WorkTaskStateId))
                .ForPath(destination => destination.WorkTaskStatusNavigation!.Code, opt => opt.MapFrom(src => src.WorkTaskStateObj!.Code))
                .ForPath(destination => destination.WorkTaskStatusNavigation!.Name, opt => opt.MapFrom(src => src.WorkTaskStateObj!.Name))
                .ForPath(destination => destination.WorkTaskStatusNavigation!.Description, opt => opt.MapFrom(src => src.WorkTaskStateObj!.Description))

                // Mapeo de AssignedToUserNavigation! (usuario asignado)
                .ForPath(destination => destination.AssignedToUserNavigation!.Id, opt => opt.MapFrom(src => src.UserAssignedObj!.UserId))
                .ForPath(destination => destination.AssignedToUserNavigation!.FirstName, opt => opt.MapFrom(src => src.UserAssignedObj!.FirstName))
                .ForPath(destination => destination.AssignedToUserNavigation!.LastName, opt => opt.MapFrom(src => src.UserAssignedObj!.LastName))

                // Mapeo de CreatedByUserNavigation! (usuario creador)
                .ForPath(destination => destination.CreatedByUserNavigation!.Id, opt => opt.MapFrom(src => src.UserByCreatedObj!.UserId))
                .ForPath(destination => destination.CreatedByUserNavigation!.FirstName, opt => opt.MapFrom(src => src.UserByCreatedObj!.FirstName))
                .ForPath(destination => destination.CreatedByUserNavigation!.LastName, opt => opt.MapFrom(src => src.UserByCreatedObj!.LastName))

                .ReverseMap();

            CreateMap<NewCreateWorkTaskDto, WorkTask>()
                //Basic
                .ForMember(destination => destination.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(destination => destination.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(destination => destination.TaskId, opt => opt.MapFrom(src => src.WorkTaskId))

                //WorkTaskStatus
                .ForPath(destination => destination.WorkTaskStatusNavigation!.WorkTaskStatusId, opt => opt.MapFrom(src => src.WorkTaskStatusId))
                .ForPath(destination => destination.WorkTaskStatusNavigation!.Name, opt => opt.MapFrom(src => src.WorkTaskStatusName))
                .ForPath(destination => destination.WorkTaskStatusNavigation!.Code, opt => opt.MapFrom(src => src.WorkTaskStatusCode))

                //AssignedToUser
                .ForPath(destination => destination.AssignedToUserNavigation!.Id, opt => opt.MapFrom(src => src.UserAssignedId))
                .ForPath(destination => destination.AssignedToUserNavigation!.FirstName, opt => opt.MapFrom(src => src.UserAssignedFirsName))
                .ForPath(destination => destination.AssignedToUserNavigation!.LastName, opt => opt.MapFrom(src => src.UserAssignedLastName))

                .ReverseMap();
        }
    }
}
