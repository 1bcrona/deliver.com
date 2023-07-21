using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeliverCom.API.Controllers.Infrastructure;
using DeliverCom.API.Model;
using DeliverCom.API.Model.Auth;
using DeliverCom.API.Model.LoginUser;
using DeliverCom.Application.Command;
using DeliverCom.Core.Helper;
using DeliverCom.Domain.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DeliverCom.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        private readonly JwtSettings _jwtOptions;

        public AuthController(IMediator mediator, IOptions<JwtSettings> jwtOptions) : base(mediator)
        {
            _jwtOptions = jwtOptions.Value;
        }

        private static TypeAdapterConfig LoginUserResponseDtoMapperConfig
        {
            get
            {
                return TypeAdapterConfig.GlobalSettings.Fork(cfg =>
                {
                    cfg.ForType<User, LoginUserResponseDto>()
                        .Map(dto => dto.UserId, entity => entity.EntityId)
                        .Map(dto => dto.Email, entity => entity.Email)
                        .Map(dto => dto.FirstName, entity => entity.FirstName)
                        .Map(dto => dto.Surname, entity => entity.Surname)
                        .Map(dto => dto.CompanyId, entity => entity.Company.EntityId);
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Post(LoginUserRequestDto requestDto)
        {
            var result = await Mediator.Send(new LoginUserCommand
            {
                Email = requestDto.Email,
                Password = requestDto.Password.Encrypt()
            });

            var loginResponseDto = result.Result.Adapt<LoginUserResponseDto>(LoginUserResponseDtoMapperConfig);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Sid, loginResponseDto.UserId),
                new Claim(ClaimTypes.Email, loginResponseDto.Email),
                new Claim(ClaimTypes.GivenName, loginResponseDto.FirstName),
                new Claim(ClaimTypes.Surname, loginResponseDto.Surname),
                new Claim(ClaimTypes.Role, loginResponseDto.Role.ToString()),
                new Claim("CompanyId", loginResponseDto.CompanyId ?? "")
            };
            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Issuer,
                claims,
                expires:
                DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var response = new ApiResponse<AuthResponseDto>
            {
                Data = new AuthResponseDto { Token = tokenString }
            };
            return await ReturnResult(response);
        }
    }
}