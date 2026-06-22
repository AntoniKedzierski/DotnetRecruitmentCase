using AutoMapper;
using CommonObjects.Invoices;
using InvoicesDatabase.Entities;

namespace DotnetRecruitmentCase.Mappers;

public class InvoicesProfile : Profile {

    public InvoicesProfile() {
        // ── Item ─────────────────────────────────────────────────────────────
        CreateMap<Item, ItemDto>();
        CreateMap<ItemDto, Item>()
            .ForMember(dest => dest.Positions, opt => opt.Ignore());

        // ── Contractor ───────────────────────────────────────────────────────
        CreateMap<Contractor, ContractorDto>();
        CreateMap<ContractorDto, Contractor>()
            .ForMember(dest => dest.Invoices, opt => opt.Ignore());

        // ── InvoicePosition ──────────────────────────────────────────────────
        CreateMap<InvoicePosition, InvoicePositionDto>();
        CreateMap<InvoicePositionDto, InvoicePosition>()
            .ForMember(dest => dest.Id,        opt => opt.Ignore())
            .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
            .ForMember(dest => dest.Invoice,   opt => opt.Ignore())
            .ForMember(dest => dest.ItemId,    opt => opt.MapFrom(src => src.Item.Id));

        // ── Invoice ──────────────────────────────────────────────────────────
        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.Positions, opt => opt.MapFrom(src => src.Positions));
        CreateMap<InvoiceDto, Invoice>()
            .ForMember(dest => dest.ContractorId, opt => opt.MapFrom(src => src.Contractor.Id))
            .ForMember(dest => dest.Contractor,   opt => opt.Ignore())
            .ForMember(dest => dest.Positions,    opt => opt.Ignore());
    }
}
