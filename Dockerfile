# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copia csproj e restaura dependências (cache)
COPY *.csproj .
RUN dotnet restore

# copia todo o código e publica
COPY . .
RUN dotnet publish -c Release -o /app

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SteamItemSeller.dll"]
EXPOSE 80
