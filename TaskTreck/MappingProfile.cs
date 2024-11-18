using AutoMapper;
using Entities.Models;
using Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TaskTreck;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Project, ProjectDto>();
        CreateMap<TaskItem, TaskItemDto>();
        CreateMap<ProjectForCreationDto, Project>();
        CreateMap<TaskItemForCreationDto, TaskItem>();
        CreateMap<TaskItemForUpdateDto, TaskItem>().ReverseMap();
        CreateMap<ProjectForUpdateDto, Project>();
    }
}