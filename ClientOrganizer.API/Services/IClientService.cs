using ClientOrganizer.API.Models.Dtos;

namespace ClientOrganizer.API.Services;

public interface IClientService
{
    Task<IEnumerable<ClientReadDto>> GetClientsAsync(int page, int pageSize);
    Task<ClientReadDto?> GetClientByIdAsync(int id);
    Task<ClientReadDto?> CreateClientAsync(ClientCreateDto createDto);
    Task<bool> UpdateClientAsync(int id, ClientUpdateDto updateDto);
    Task<bool> DeleteClientAsync(int id);
}
