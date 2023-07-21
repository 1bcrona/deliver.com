using DeliverCom.API.Controllers.Infrastructure;
using DeliverCom.API.Model;
using DeliverCom.API.Model.RegisterCompany;
using DeliverCom.Application.Command;
using DeliverCom.Domain.Company;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverCom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : BaseController
    {
        public CompanyController(IMediator mediator) : base(mediator)
        {
        }

        private static TypeAdapterConfig RegisterCompanyResponseDtoMapperConfig
        {
            get
            {
                return TypeAdapterConfig.GlobalSettings.Fork(cfg =>
                {
                    cfg.ForType<Company, RegisterCompanyResponseDto>()
                        .Map(dto => dto.CompanyId, entity => entity.EntityId)
                        .Map(dto => dto.Name, entity => entity.Name);
                });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize("SystemAdmin")]
        public async Task<IActionResult> Post(RegisterCompanyRequestDto companyRequestDto)
        {
            var result = await Mediator.Send(new RegisterCompanyCommand
            {
                Name = companyRequestDto.Name
            });

            var companyResponseDto =
                result.Result.Adapt<RegisterCompanyResponseDto>(RegisterCompanyResponseDtoMapperConfig);

            var resultObject = new ApiResponse<RegisterCompanyResponseDto>
            {
                Data = companyResponseDto
            };
            return await ReturnResult(resultObject);
        }
    }
}