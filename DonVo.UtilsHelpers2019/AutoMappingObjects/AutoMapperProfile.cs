using AutoMapper;
using AutoMappingObjects.DTOs;
using AutoMappingObjects.Models;

namespace AutoMappingObjects
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDto, Employee>();
            CreateMap<Employee, EmployeePersonalInfoDto>();
        }
    }
}
