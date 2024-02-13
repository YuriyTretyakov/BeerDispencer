#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BeerDispencer.Host/BeerDispenser.WebApi.csproj", "BeerDispencer.Host/"]
COPY ["BeerDispencer.Infrastructure/BeerDispencer.Infrastructure.csproj", "BeerDispencer.Infrastructure/"]
COPY ["BeerDispencer.Application/BeerDispenser.Application.csproj", "BeerDispencer.Application/"]
COPY ["BeerDispencer.Domain/BeerDispenser.Domain.csproj", "BeerDispencer.Domain/"]
COPY ["BeerDispencer.Shared/BeerDispenser.Shared.csproj", "BeerDispencer.Shared/"]
COPY ["BeerDispenser.Messaging/BeerDispenser.Messaging.csproj", "BeerDispenser.Messaging/"]
RUN dotnet restore "BeerDispencer.Host/BeerDispenser.WebApi.csproj"
COPY . .
WORKDIR "/src/BeerDispencer.Host"
RUN dotnet build "BeerDispenser.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeerDispenser.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeerDispenser.WebApi.dll"]
