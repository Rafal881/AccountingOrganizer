using System.ComponentModel.DataAnnotations;

namespace ClientOrganizer.API.Models.Dtos
{
    public class ClientUpdateDto
    {
        public string? Name { get; set; }

        public string? Address { get; set; }

        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Nip must be 10 digits")]
        public string? NipNb { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
