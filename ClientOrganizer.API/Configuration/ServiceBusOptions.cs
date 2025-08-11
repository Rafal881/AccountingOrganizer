using System.ComponentModel.DataAnnotations;

namespace ClientOrganizer.API.Configuration;

public class ServiceBusOptions
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string QueueName { get; set; } = string.Empty;
}
