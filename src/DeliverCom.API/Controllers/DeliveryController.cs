using DeliverCom.API.Controllers.Infrastructure;
using DeliverCom.API.Model;
using DeliverCom.API.Model.CreateDelivery;
using DeliverCom.API.Model.GetDeliveries;
using DeliverCom.Application.Command;
using DeliverCom.Application.Query;
using DeliverCom.Domain.Delivery;
using DeliverCom.Domain.Delivery.Contracts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverCom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeliveryController : BaseController
    {
        public DeliveryController(IMediator mediator) : base(mediator)
        {
        }

        private static TypeAdapterConfig DeliveryQueryResponseDtoMapperConfig
        {
            get
            {
                return TypeAdapterConfig.GlobalSettings.Fork(cfg =>
                {
                    cfg.ForType<PaginatedDeliveriesContract, GetDeliveriesResponseDto>()
                        .Map(dto => dto.Deliveries, entity => entity.Deliveries)
                        .Map(dto => dto.TotalCount, entity => entity.TotalCount)
                        .Map(dto => dto.PageNumber, entity => entity.PageNumber)
                        .Map(dto => dto.PageSize, entity => entity.PageSize);
                });
            }
        }

        private static TypeAdapterConfig CreateDeliveryResponseDtoMapperConfig
        {
            get
            {
                return TypeAdapterConfig.GlobalSettings.Fork(cfg =>
                {
                    cfg.ForType<Delivery, CreateDeliveryResponseDto>()
                        .Map(dto => dto.CompanyId, entity => entity.Company.EntityId)
                        .Map(dto => dto.DeliveryId, entity => entity.EntityId)
                        .Map(dto => dto.DeliveryNumber, entity => entity.DeliveryNumber)
                        .Map(dto => dto.Status, entity => entity.DeliveryStatus);
                });
            }
        }

        [HttpGet]
        [Route("")]
        [Authorize("CompanyUser")]
        public async Task<IActionResult> Get([FromQuery] GetDeliveriesRequestDto requestDto,
            [FromQuery] PagingRequestDto pagingRequestDto)
        {
            var result = await Mediator.Send(new GetDeliveriesQuery
            {
                SenderCity = requestDto.SenderCity,
                SenderTown = requestDto.SenderTown,
                DeliveryCity = requestDto.DeliveryCity,
                DeliveryTown = requestDto.DeliveryTown,
                DeliveryStatus = requestDto.DeliveryStatus,
                DeliveryId = requestDto.DeliveryId,
                DeliveryNumber = requestDto.DeliveryNumber,
                PageSize = pagingRequestDto.PageSize,
                PageNumber = pagingRequestDto.PageNumber
            });

            var deliveryQueryResponseDto =
                result.Result.Adapt<GetDeliveriesResponseDto>(DeliveryQueryResponseDtoMapperConfig);

            var resultObject = new ApiResponse<GetDeliveriesResponseDto>
            {
                Data = deliveryQueryResponseDto
            };

            return await ReturnResult(resultObject);
        }


        [HttpPost]
        [Route("")]
        [Authorize("CompanyUser")]
        public async Task<IActionResult> Post(CreateDeliverRequestDto requestDto)
        {
            var result = await Mediator.Send(new CreateDeliveryCommand
            {
                CompanyId = requestDto.CompanyId,
                SenderAddressCity = requestDto.SenderAddress.City,
                SenderAddressStreet = requestDto.SenderAddress.Street,
                SenderAddressTown = requestDto.SenderAddress.Town,
                SenderAddressZipCode = requestDto.SenderAddress.ZipCode,
                DeliveryAddressCity = requestDto.DeliveryAddress.City,
                DeliveryAddressStreet = requestDto.DeliveryAddress.Street,
                DeliveryAddressTown = requestDto.DeliveryAddress.Town,
                DeliveryAddressZipCode = requestDto.DeliveryAddress.ZipCode
            });

            var createDeliveryResponse =
                result.Result.Adapt<CreateDeliveryResponseDto>(CreateDeliveryResponseDtoMapperConfig);

            var resultObject = new ApiResponse<CreateDeliveryResponseDto>
            {
                Data = createDeliveryResponse
            };

            return await ReturnResult(resultObject);
        }
    }
}