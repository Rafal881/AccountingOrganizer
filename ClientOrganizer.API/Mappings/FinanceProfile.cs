using AutoMapper;
using ClientOrganizer.API.Models.Dtos;
using ClientOrganizer.API.Models.Entities;

namespace ClientOrganizer.API.Mappings;

public class FinanceProfile : Profile
{
    public FinanceProfile()
    {
        CreateMap<FinancialData, FinancialRecordReadDto>();
        CreateMap<FinancialRecordCreateDto, FinancialData>();
        CreateMap<FinancialRecordUpdateDto, FinancialData>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
