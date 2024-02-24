FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

#COPY . ./
#COPY BeerDispencer.sln ./
COPY ["BeerDispencer.WebUi/BeerDispenser.WebUi.csproj", "/BeerDispencer.WebUi/"]
COPY ["BeerDispencer.WebUi/wwwroot", "/BeerDispencer.WebUi/wwwroot"]
COPY ["BeerDispencer.Shared/BeerDispenser.Shared.csproj", "/BeerDispencer.Shared/"]

#WORKDIR /app/BeerDispencerWebUi
#WORKDIR /app
RUN dotnet restore /BeerDispencer.WebUi/BeerDispenser.WebUi.csproj
COPY . ./

RUN dotnet build /BeerDispencer.WebUi/BeerDispenser.WebUi.csproj -c Release -o /app/build


FROM build AS publish
RUN dotnet publish /BeerDispencer.WebUi/BeerDispenser.WebUi.csproj -c Release -o /app/publish 

FROM nginx:1.23.0-alpine
WORKDIR /usr/share/nginx/html
EXPOSE 8080
#COPY nginx.conf /etc/nginx/nginx.conf
#COPY --from=build /app/out/wwwroot /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf