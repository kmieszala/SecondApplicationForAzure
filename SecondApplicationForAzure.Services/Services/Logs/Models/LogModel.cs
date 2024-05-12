using SecondApplicationForAzure.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SecondApplicationForAzure.Services.Services.Logs.Models;

public record LogModel(EventTypeEnum LogType, [Required] string Message);