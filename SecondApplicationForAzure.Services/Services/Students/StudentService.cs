using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SecondApplicationForAzure.Model;
using SecondApplicationForAzure.Model.DbSets;
using SecondApplicationForAzure.Services.Services.Students.Models;

namespace SecondApplicationForAzure.Services.Services.Students;

public interface IStudentService
{
    Task<StudentModel> AddStudentAsync(string name);

    Task<StudentModel> EditStudentAsync(StudentModel studentModel);

    Task<List<StudentModel>> GetStudentsAsync();
}

public class StudentService : IStudentService
{
    private readonly SecondAppDbContext _context;
    private readonly IMapper _mapper;

    public StudentService(SecondAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<StudentModel> AddStudentAsync(string name)
    {
        var student = new Student() { Name = name };
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        return _mapper.Map<StudentModel>(student);
    }

    public async Task<StudentModel> EditStudentAsync(StudentModel studentModel)
    {
        var studentDb = await _context.Students
            .Where(x => x.Id == studentModel.Id)
            .FirstOrDefaultAsync();

        if (studentDb == null)
        {
            return await AddStudentAsync(studentModel.Name);
        }

        studentDb.Name = studentModel.Name;
        await _context.SaveChangesAsync();

        return studentModel;
    }

    public async Task<List<StudentModel>> GetStudentsAsync()
    {
        var studentsList = await _context.Students.ToListAsync();
        return _mapper.Map<List<StudentModel>>(studentsList);
    }
}