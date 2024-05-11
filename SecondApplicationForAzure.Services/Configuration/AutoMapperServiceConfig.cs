using AutoMapper;
using SecondApplicationForAzure.Model.DbSets;
using SecondApplicationForAzure.Services.Services.Students.Models;

namespace SecondApplicationForAzure.Services.Configuration;

public class AutoMapperServiceConfig : Profile
{
    public AutoMapperServiceConfig()
    {
        CreateMap<StudentModel, Student>();
        CreateMap<Student, StudentModel>();
    }
}