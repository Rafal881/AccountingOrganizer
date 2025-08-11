using AutoMapper;
using ClientOrganizer.API.Data;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClientOrganizer.API.Services;

public class ClientService : IClientService
{
    private readonly ClientOrganizerDbContext _dbContext;
    private readonly IMapper _mapper;

    public ClientService(ClientOrganizerDbContext db, IMapper mapper)
    {
        _dbContext = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClientReadDto>> GetClientsAsync()
    {
        var clients = await _dbContext.Clients.ToListAsync();
        return _mapper.Map<IEnumerable<ClientReadDto>>(clients);
    }

    public async Task<ClientReadDto?> GetClientByIdAsync(int id)
    {
        var client = await _dbContext.Clients.FindAsync(id);
        return client is null ? null : _mapper.Map<ClientReadDto>(client);
    }

    public async Task<ClientReadDto?> CreateClientAsync(ClientCreateDto createDto)
    {
        var exists = await _dbContext.Clients.AnyAsync(c => c.NipNb == createDto.NipNb);
        if (exists) return null;

        var client = _mapper.Map<Client>(createDto);
        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<ClientReadDto>(client);
    }

    public async Task<bool> UpdateClientAsync(int id, ClientUpdateDto updateDto)
    {
        var client = await _dbContext.Clients.FindAsync(id);
        if (client is null) return false;

        _mapper.Map(updateDto, client);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _dbContext.Clients.FindAsync(id);
        if (client is null) return false;

        _dbContext.Clients.Remove(client);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
