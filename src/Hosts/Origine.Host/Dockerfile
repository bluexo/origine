FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Origine.Host/Origine.Host.csproj", "src/Origine.Host/"]
COPY ["src/Origine.Core.Common/Origine.Core.Common.csproj", "src/Origine.Core.Common/"]
COPY ["src/Origine.Core.Abstraction/Origine.Core.Abstraction.csproj", "src/Origine.Core.Abstraction/"]
COPY ["src/Origine.Core.Data/Origine.Core.Data.csproj", "src/Origine.Core.Data/"]
COPY ["src/Origine.Grains.MatchMaker/Origine.Grains.MatchMaker.csproj", "src/Origine.Grains.MatchMaker/"]
COPY ["src/Origine.Interfaces/Origine.Interfaces.csproj", "src/Origine.Interfaces/"]
COPY ["src/Origine.Grains.Abstraction/Origine.Grains.Abstraction.csproj", "src/Origine.Grains.Abstraction/"]
COPY ["src/Origine.Grains.PlayGround/Origine.Grains.PlayGround.csproj", "src/Origine.Grains.PlayGround/"]
COPY ["src/Origine.Grains.AI/Origine.Grains.AI.csproj", "src/Origine.Grains.AI/"]
COPY ["src/Origine.Grains.Lobby/Origine.Grains.Lobby.csproj", "src/Origine.Grains.Lobby/"]
COPY ["src/Origine.Storage.EntityFramework/Origine.Storage.EF.csproj", "src/Origine.Storage.EntityFramework/"]
COPY ["src/Origine.Storage.Accessor/Origine.Storage.Accessor.csproj", "src/Origine.Storage.Accessor/"]
COPY ["src/Origine.Grains.Infrastructure/Origine.Grains.Infrastructure.csproj", "src/Origine.Grains.Infrastructure/"]
RUN dotnet restore "src/Origine.Host/Origine.Host.csproj"
COPY . .
WORKDIR "/src/src/Origine.Host"
RUN dotnet build "Origine.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Origine.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Origine.Host.dll"]