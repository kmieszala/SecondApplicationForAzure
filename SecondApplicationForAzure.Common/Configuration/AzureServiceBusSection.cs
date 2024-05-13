using System.ComponentModel.DataAnnotations;

namespace SecondApplicationForAzure.Common.Configuration;

public class AzureServiceBusSection
{
    public const string SectionName = "AzureServiceBus";

    [Required]
    public required string SendNamespaceConnectionString { get; init; }

    [Required]
    public required string ListenNamespaceConnectionString { get; init; }

    [Required]
    public required string QueueName { get; init; }
}