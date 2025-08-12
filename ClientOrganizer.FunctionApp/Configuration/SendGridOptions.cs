using System.ComponentModel.DataAnnotations;

namespace ClientOrganizer.FunctionApp.Configuration;

public class SendGridOptions
{
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string FromEmail { get; set; } = string.Empty;
}
