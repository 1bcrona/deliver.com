using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DeliverCom.API.Extensions;
using DeliverCom.API.Filter;
using DeliverCom.API.Middlewares;
using DeliverCom.API.Model.Address;
using DeliverCom.API.Model.Company;
using DeliverCom.API.Model.Delivery;
using DeliverCom.Application.Extension;
using DeliverCom.Container.Autofac;
using DeliverCom.Core.Routing.Settings;
using DeliverCom.Domain.Company;
using DeliverCom.Domain.Context;
using DeliverCom.Domain.Delivery;
using DeliverCom.Domain.Delivery.ValueObject;
using DeliverCom.Domain.User.ValueObject;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Savorboard.CAP.InMemoryMessageQueue;

namespace DeliverCom.API
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", false, true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true,
                true);
            builder.Configuration.AddEnvironmentVariables();
            builder.Services.AddLogger(configurationBuilder =>
            {
                configurationBuilder.AddJsonFile("serilog.json", false, true);
                configurationBuilder.AddEnvironmentVariables("Logging_");
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CompanyUser",
                    policy => policy.RequireClaim(ClaimTypes.Role, UserRole.COMPANY_USR.ToString()));
                options.AddPolicy("SystemAdmin",
                    policy => policy.RequireClaim(ClaimTypes.Role, UserRole.SYSTEM_ADM.ToString()));
            });


            builder.Services.AddOptions<RouteSettings>().BindConfiguration("RouteSettings");
            builder.Services.AddOptions<JwtSettings>().BindConfiguration("JwtSettings");

            var jwtSettings = builder.Configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            builder.Services.AddControllers(options => { options.Filters.Add(typeof(AuthenticationFilter)); })
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddCap(provider =>
            {
                provider.UseEntityFramework<DeliverComDbContext>();
                provider.UsePostgreSql(builder.Configuration.GetConnectionString("DeliverComDb"));
                provider.UseInMemoryMessageQueue();
                provider.UseDashboard();
                provider.SucceedMessageExpiredAfter = 24 * 3600;
            });

            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "deliver.com API",
                    Version = "v1"
                });
                c.SchemaFilter<NamespaceSchemaFilter>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.SchemaFilter<DescribeEnumMembersFilter>(xmlPath);
                c.CustomSchemaIds(customSchemaIds => customSchemaIds.FullName);
                c.IncludeXmlComments(xmlPath);
            });


            builder.Services.AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    authenticationOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(jwtSettings.Key)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        SaveSigninToken = true
                    };
                });

            TypeAdapterConfig.GlobalSettings.ForType<Address, AddressDto>()
                .Map(dto => dto.City, entity => entity.City)
                .Map(dto => dto.Street, entity => entity.Street)
                .Map(dto => dto.Town, entity => entity.Town)
                .Map(dto => dto.ZipCode, entity => entity.ZipCode);


            TypeAdapterConfig.GlobalSettings.ForType<Company, CompanyDto>()
                .Map(dto => dto.CompanyId, entity => entity.EntityId)
                .Map(dto => dto.Name, entity => entity.Name);

            TypeAdapterConfig.GlobalSettings.ForType<Delivery, DeliveryDto>()
                .Map(dto => dto.Company, entity => entity.Company)
                .Map(dto => dto.DeliveryId, entity => entity.EntityId)
                .Map(dto => dto.Status, entity => entity.DeliveryStatus)
                .Map(dto => dto.SenderAddress, entity => entity.SenderAddress)
                .Map(dto => dto.DeliveryAddress, entity => entity.DeliveryAddress)
                .Map(dto => dto.DeliveryNumber, entity => entity.DeliveryNumber);

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<AutofacContainerBuilder>(container =>
                {
                    container.Bootstrap(builder.Configuration);
                });
            var app = builder.Build();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });

            using (var serviceScope = app.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DeliverComDbContext>();
                var databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
                databaseCreator.EnsureCreated();
                try
                {
                    databaseCreator.CreateTables();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            app.UseMiddleware(typeof(RequestMiddleware));
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeliverCom v1"));
            app.UseRouting();
            app.UseAuthentication(); // This line must be immediately after UseRouting.
            app.UseAuthorization(); // This line must be immediately after UseAuthentication. This line must be appear between UseRouting and UseEndpoints.

            app.MapControllers();
            app.Run();
        }
    }
}