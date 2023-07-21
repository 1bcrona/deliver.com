FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/DeliverCom.API/DeliverCom.API.csproj", "src/DeliverCom.API/"]
COPY ["src/DeliverCom.Core/DeliverCom.Core.csproj", "src/DeliverCom.Core/"]
COPY ["src/DeliverCom.Container/DeliverCom.Container.csproj", "src/DeliverCom.Container/"]
COPY ["src/DeliverCom.Application/DeliverCom.Application.csproj", "src/DeliverCom.Application/"]
COPY ["src/DeliverCom.Routing/DeliverCom.Routing.csproj", "src/DeliverCom.Routing/"]
COPY ["src/DeliverCom.Resolver/DeliverCom.Resolver.csproj", "src/DeliverCom.Resolver/"]
COPY ["src/DeliverCom.Domain/DeliverCom.Domain.csproj", "src/DeliverCom.Domain/"]
COPY ["src/DeliverCom.UseCase/DeliverCom.UseCase.csproj", "src/DeliverCom.UseCase/"]
COPY ["src/DeliverCom.Data/DeliverCom.Data.csproj", "src/DeliverCom.Data/"]
COPY ["test/DeliverCom.Test/DeliverCom.Test.csproj", "test/DeliverCom.Test/"]
RUN dotnet restore "src/DeliverCom.API/DeliverCom.API.csproj"
RUN dotnet restore "test/DeliverCom.Test/DeliverCom.Test.csproj"
COPY . .
WORKDIR "/src/src/DeliverCom.API"
RUN dotnet build "DeliverCom.API.csproj" -c Release -o /app/build
WORKDIR "/src/test/DeliverCom.Test"
RUN dotnet test

FROM build AS publish
WORKDIR "/src/src/DeliverCom.API"
RUN dotnet publish "DeliverCom.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliverCom.API.dll"]
