FROM nginx AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BeerDispencer.WebUi/BeerDispenser.WebUi.csproj", "BeerDispencer.WebUi/"]
COPY ["BeerDispencer.Shared/BeerDispenser.Shared.csproj", "BeerDispencer.Shared/"]


RUN dotnet restore "BeerDispencer.WebUi/BeerDispenser.WebUi.csproj"

COPY . .
WORKDIR "/src/BeerDispencer.WebUi"
RUN dotnet build "BeerDispenser.WebUi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BeerDispenser.WebUi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM  base AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
