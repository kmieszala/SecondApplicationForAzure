using System.ComponentModel.DataAnnotations;

namespace SecondApplicationForAzure.Common.Configuration;

public class AzureStorageBlobSection
{
    public const string SectionName = "AzureStorageBlobSection";

    [Required]
    public required string ConnectionString { get; init; }

    [Required]
    public required string BlobContainerName { get; set; }
}