FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Origine.WebApi/Origine.WebApi.csproj", "src/Origine.WebApi/"]
COPY ["src/Origine.Core.Common/Origine.Core.Common.csproj", "src/Origine.Core.Common/"]
COPY ["src/Origine.Core.Abstraction/Origine.Core.Abstraction.csproj", "src/Origine.Core.Abstraction/"]
COPY ["src/Origine.Core.Data/Origine.Core.Data.csproj", "src/Origine.Core.Data/"]
COPY ["src/Origine.Storage.EntityFramework/Origine.Storage.EF.csproj", "src/Origine.Storage.EntityFramework/"]
COPY ["src/Origine.Interfaces/Origine.Interfaces.csproj", "src/Origine.Interfaces/"]
COPY ["src/Origine.Storage.Accessor/Origine.Storage.Accessor.csproj", "src/Origine.Storage.Accessor/"]
RUN dotnet restore "src/Origine.WebApi/Origine.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Origine.WebApi"
RUN dotnet build "Origine.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Origine.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Origine.WebApi.dll"]