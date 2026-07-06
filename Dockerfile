# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Reinge-SistemaInventarioLegacy/Reinge-SistemaInventarioLegacy.csproj", "Reinge-SistemaInventarioLegacy/"]
RUN dotnet restore "Reinge-SistemaInventarioLegacy/Reinge-SistemaInventarioLegacy.csproj"

COPY . .
RUN dotnet publish "Reinge-SistemaInventarioLegacy/Reinge-SistemaInventarioLegacy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Reinge-SistemaInventarioLegacy.dll"]