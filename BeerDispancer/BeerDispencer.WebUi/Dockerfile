#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
RUN ls
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BeerDispencer.WebUi/BeerDispenser.WebUi.csproj", "BeerDispencer.WebUi/"]
COPY ["BeerDispencer.Shared/BeerDispenser.Shared.csproj", "BeerDispencer.Shared/"]
RUN dotnet restore "BeerDispencer.WebUi/BeerDispenser.WebUi.csproj"
COPY . .
WORKDIR "/src/BeerDispencer.WebUi"
RUN dotnet build "BeerDispenser.WebUi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeerDispenser.WebUi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeerDispenser.WebUi.dll"]
