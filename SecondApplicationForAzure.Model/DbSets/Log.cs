using SecondApplicationForAzure.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SecondApplicationForAzure.Model.DbSets;

public class Log
{
    [Key]
    public Guid Id { get; set; }

    public string? Message { get; set; }

    public required string ApppName { get; set; }

    public required string MessageId { get; set; }

    public EventTypeEnum LogType { get; set; }

    /// <summary>
    /// Event creation time
    /// </summary>
    public DateTime EventDate { get; set; }

    /// <summary>
    /// Record creation time
    /// </summary>
    public DateTime CreateDate { get; set; }
}