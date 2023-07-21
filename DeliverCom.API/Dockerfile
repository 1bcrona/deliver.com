FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DeliverCom.API/DeliverCom.API.csproj", "DeliverCom.API/"]
COPY ["DeliverCom.Core/DeliverCom.Core.csproj", "DeliverCom.Core/"]
COPY ["DeliverCom.Container/DeliverCom.Container.csproj", "DeliverCom.Container/"]
COPY ["DeliverCom.Application/DeliverCom.Application.csproj", "DeliverCom.Application/"]
COPY ["DeliverCom.Routing/DeliverCom.Routing.csproj", "DeliverCom.Routing/"]
COPY ["DeliverCom.Resolver/DeliverCom.Resolver.csproj", "DeliverCom.Resolver/"]
COPY ["DeliverCom.Domain/DeliverCom.Domain.csproj", "DeliverCom.Domain/"]
COPY ["DeliverCom.UseCase/DeliverCom.UseCase.csproj", "DeliverCom.UseCase/"]
COPY ["DeliverCom.Repository/DeliverCom.Repository.csproj", "DeliverCom.Repository/"]
COPY ["DeliverCom.Data/DeliverCom.Data.csproj", "DeliverCom.Data/"]
COPY ["DeliverCom.Test/DeliverCom.Test.csproj", "DeliverCom.Test/"]
RUN dotnet restore "DeliverCom.API/DeliverCom.API.csproj"
RUN dotnet restore "DeliverCom.Test/DeliverCom.Test.csproj"
COPY . .
WORKDIR "/src/DeliverCom.API"
RUN dotnet build "DeliverCom.API.csproj" -c Release -o /app/build
WORKDIR "/src/DeliverCom.Test"
RUN dotnet test

FROM build AS publish
WORKDIR "/src/DeliverCom.API"
RUN dotnet publish "DeliverCom.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliverCom.API.dll"]
