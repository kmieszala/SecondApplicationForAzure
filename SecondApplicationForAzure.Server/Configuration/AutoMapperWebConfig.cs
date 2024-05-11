using AutoMapper;
using SecondApplicationForAzure.Server.Controllers.Students.ViewModels;
using SecondApplicationForAzure.Services.Services.Students.Models;

namespace SecondApplicationForAzure.Server.Configuration;

public class AutoMapperWebConfig : Profile
{
    public AutoMapperWebConfig()
    {
        CreateMap<StudentVM, StudentModel>();
        CreateMap<StudentModel, StudentVM>();
    }
}