namespace ClientOrganizer.API.Models.Dtos
{
    public class ClientCreateDto
    {
        public required string Name { get; set; }
        public string Address { get; set; }
        public string NipNb { get; set; }
        public string Email { get; set; }
    }
}
