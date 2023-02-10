using AutoMapper;
using Order_Service.Entities.Dtos;
using Order_Service.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order_Service.Mappers
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<WishListProductDTO, ProductDTO>().ReverseMap().ForMember(sel => sel.Qunatity, act => act.MapFrom(sel => ""));
            CreateMap<OrderResponseDTO, Bill>().ReverseMap().ForMember(sel => sel.BillId, act => act.MapFrom(sel => sel.Id))
               // .ForMember(sel => sel.Product, act => act.MapFrom(sel => new Product()))
                ;
        }
    }
}
