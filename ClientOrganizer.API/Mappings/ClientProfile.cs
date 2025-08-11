using AutoMapper;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;

namespace ClientOrganizer.API.Mappings;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<Client, ClientReadDto>();
        CreateMap<ClientCreateDto, Client>();
        CreateMap<ClientUpdateDto, Client>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
