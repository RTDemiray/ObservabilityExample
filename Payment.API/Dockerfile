﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Payment.API/Payment.API.csproj", "Payment.API/"]
COPY ["Log.Shared/Log.Shared.csproj", "Log.Shared/"]
COPY ["Common.Shared/Common.Shared.csproj", "Common.Shared/"]
COPY ["OpenTelemetry.Shared/OpenTelemetry.Shared.csproj", "OpenTelemetry.Shared/"]
RUN dotnet restore "Payment.API/Payment.API.csproj"
COPY . .
WORKDIR "/src/Payment.API"
RUN dotnet build "Payment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.API.dll"]
