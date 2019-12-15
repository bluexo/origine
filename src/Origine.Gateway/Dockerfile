FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/Origine.Gateway/Origine.Gateway.csproj", "src/Origine.Gateway/"]
COPY ["src/Origine.Models/Origine.Models.csproj", "src/Origine.Models/"]
COPY ["src/Origine.Interfaces/Origine.Interfaces.csproj", "src/Origine.Interfaces/"]
COPY ["src/Origine.Common/Origine.Common.csproj", "src/Origine.Common/"]
COPY ["src/Origine.StorageProviders/Origine.StorageProviders.csproj", "src/Origine.StorageProviders/"]
RUN dotnet restore "src/Origine.Gateway/Origine.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Origine.Gateway"
RUN dotnet build "Origine.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Origine.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Origine.Gateway.dll"]