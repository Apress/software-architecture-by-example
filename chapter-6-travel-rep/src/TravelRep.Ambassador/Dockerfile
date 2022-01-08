#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

#COPY ["localhost.crt", "localhost.crt"]
#COPY ["localhost.pfx", "localhost.pfx"]

#RUN ["cp", "localhost.crt", "/usr/local/share/ca-certificates"]
#RUN update-ca-certificates

#RUN dotnet dev-certs https --trust
#RUN dotnet dev-certs https --clean --import ./localhost.pfx -p <password>
#RUN dotnet dev-certs https --clean --import ./host.docker.pfx -p user

#RUN dotnet dev-certs https
#RUN dotnet dev-certs https -ep .\localhost.crt --format PEM
#RUN update-ca-certificates

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
  && mkcert -install

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelRep.Ambassador.dll"]