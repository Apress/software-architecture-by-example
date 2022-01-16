#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["TravelRep.Ambassador.csproj", "TravelRep.Ambassador/"]
RUN dotnet restore "TravelRep.Ambassador/TravelRep.Ambassador.csproj"

WORKDIR /src/TravelRep.Ambassador/
COPY . .

RUN dotnet build "TravelRep.Ambassador.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TravelRep.Ambassador.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY mkcert /usr/local/bin
COPY rootCA*.pem /root/.local/share/mkcert/
RUN chmod +x /usr/local/bin/mkcert \
  && mkcert -install \
  && rm -rf /usr/local/bin/mkcert 

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelRep.Ambassador.dll"]