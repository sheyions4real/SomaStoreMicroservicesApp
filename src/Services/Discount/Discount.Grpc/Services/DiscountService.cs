﻿using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        // this is the controller class like in REST Web API
        // this class will inherit from the DiscountGrpc generated class in bin

        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        //override the menthod GetDiscount of the DiscountProtoServiceBase generated parent class 
        // the parent class exposed the GetDiscount Method but overriding it is where the code will be implemented
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);

            if(coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
            }
            _logger.LogInformation("Discount is retrieved for ProductName: {productName}, Amount: {Amount}", new { coupon.ProductName, coupon.Amount });
            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
           // return base.GetDiscount(request, context);
        }


        // override the createDiscount method

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.CreateDiscount(coupon);
            _logger.LogInformation("Discount is Successfully created. ProductName: {ProductName}", new { coupon.ProductName });

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }




        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.UpdateDiscount(coupon);
            _logger.LogInformation("Discount is Successfully Updated . ProductName: {ProductName}", new { coupon.ProductName });

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }



        public override async  Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {

            var deleted = await _repository.DeleteDiscount(request.ProductName);

            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            _logger.LogInformation("Discount is Successfully Deleted . ProductName: {ProductName}", new { request.ProductName });

            
            return response;
        }



    }
}
