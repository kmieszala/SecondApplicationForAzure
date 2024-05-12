using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SecondApplicationForAzure.Server.Controllers.Students.ViewModels;
using SecondApplicationForAzure.Services.Services.Students;
using SecondApplicationForAzure.Services.Services.Students.Models;

namespace SecondApplicationForAzure.Server.Controllers.Students;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IMapper _mapper;

    public StudentController(IStudentService studentService, IMapper mapper)
    {
        _studentService = studentService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        var result = await _studentService.GetStudentsAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent(AddStudentVM model)
    {
        var result = await _studentService.AddStudentAsync(model.Name);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> EditStudent(StudentVM model)
    {
        var result = await _studentService.EditStudentAsync(_mapper.Map<StudentModel>(model));
        return Ok(result);
    }
}