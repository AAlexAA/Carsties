using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.DTOs;
using AutoMapper;

namespace AuctionService.RequestHelper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionDto>();
            CreateMap<CreateAuctionDto, Auction>().ForMember(x => x.Item, opt => opt.MapFrom(src => src));
            CreateMap<CreateAuctionDto, Item>();
        }
    }
}