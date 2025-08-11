using System.ComponentModel.DataAnnotations;

namespace ClientOrganizer.API.Models.Dtos
{
    public class ClientCreateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Nip must be 10 digits")]
        public string NipNb { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
