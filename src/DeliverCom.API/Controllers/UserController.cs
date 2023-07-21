using DeliverCom.API.Controllers.Infrastructure;
using DeliverCom.API.Model;
using DeliverCom.API.Model.RegisterUser;
using DeliverCom.Application.Command;
using DeliverCom.Core.Helper;
using DeliverCom.Domain.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliverCom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        public UserController(IMediator mediator) : base(mediator)
        {
        }

        private static TypeAdapterConfig RegisterUserResponseDtoMapperConfig
        {
            get
            {
                return TypeAdapterConfig.GlobalSettings.Fork(cfg =>
                {
                    cfg.ForType<User, RegisterUserResponseDto>()
                        .Map(dto => dto.UserId, entity => entity.EntityId)
                        .Map(dto => dto.FirstName, entity => entity.FirstName)
                        .Map(dto => dto.Surname, entity => entity.Surname)
                        .Map(dto => dto.Email, entity => entity.Email);
                });
            }
        }


        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Post(RegisterUserRequestDto userRequestDto)
        {
            var result = await Mediator.Send(new RegisterUserCommand
            {
                CompanyId = userRequestDto.CompanyId,
                Password = userRequestDto.Password.Encrypt(),
                EmailAddress = userRequestDto.Email,
                FirstName = userRequestDto.FirstName,
                Role = userRequestDto.Role,
                Surname = userRequestDto.Surname
            });

            var userResponseDto = result.Result.Adapt<RegisterUserResponseDto>(RegisterUserResponseDtoMapperConfig);

            var resultObject = new ApiResponse<RegisterUserResponseDto>
            {
                Data = userResponseDto
            };
            return await ReturnResult(resultObject);
        }
    }
}