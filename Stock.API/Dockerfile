FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Stock.API/Stock.API.csproj", "Stock.API/"]
COPY ["Common.Shared/Common.Shared.csproj", "Common.Shared/"]
COPY ["OpenTelemetry.Shared/OpenTelemetry.Shared.csproj", "OpenTelemetry.Shared/"]
COPY ["Log.Shared/Log.Shared.csproj", "Log.Shared/"]
RUN dotnet restore "Stock.API/Stock.API.csproj"
COPY . .
WORKDIR "/src/Stock.API"
RUN dotnet build "Stock.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stock.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stock.API.dll"]
