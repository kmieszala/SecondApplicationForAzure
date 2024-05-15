using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SecondApplicationForAzure.Services.Services.Files;
using System.ComponentModel.DataAnnotations;

namespace SecondApplicationForAzure.Server.Controllers.Files;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public FileController(IFileService fileService, IMapper mapper)
    {
        _fileService = fileService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetFilesNames()
    {
        var result = await _fileService.GetListBlobs();
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{message}")]
    public async Task<IActionResult> AddDefaultFile([FromRoute][MaxLength(50)] string message)
    {
        var result = await _fileService.AddDefaultTxtFile(message);
        return Ok(result);
    }
}