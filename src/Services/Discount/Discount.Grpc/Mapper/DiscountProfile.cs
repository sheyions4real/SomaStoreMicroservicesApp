using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Mapper
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            // use this discountProfile to create a map that converts a coupon object to a CouponModel Object
            CreateMap<Coupon, CouponModel>().ReverseMap();
        }
    }
}
