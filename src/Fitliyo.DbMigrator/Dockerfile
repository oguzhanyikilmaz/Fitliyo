FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# SSL sertifika sorununu çöz
RUN apt-get update && apt-get install -y ca-certificates && update-ca-certificates
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

COPY ["NuGet.Config", "."]
COPY ["common.props", "."]
COPY ["Fitliyo.slnx", "."]

COPY ["src/Fitliyo.Domain.Shared/Fitliyo.Domain.Shared.csproj", "src/Fitliyo.Domain.Shared/"]
COPY ["src/Fitliyo.Domain/Fitliyo.Domain.csproj", "src/Fitliyo.Domain/"]
COPY ["src/Fitliyo.Application.Contracts/Fitliyo.Application.Contracts.csproj", "src/Fitliyo.Application.Contracts/"]
COPY ["src/Fitliyo.EntityFrameworkCore/Fitliyo.EntityFrameworkCore.csproj", "src/Fitliyo.EntityFrameworkCore/"]
COPY ["src/Fitliyo.HttpApi.Client/Fitliyo.HttpApi.Client.csproj", "src/Fitliyo.HttpApi.Client/"]
COPY ["src/Fitliyo.DbMigrator/Fitliyo.DbMigrator.csproj", "src/Fitliyo.DbMigrator/"]

RUN dotnet restore "src/Fitliyo.DbMigrator/Fitliyo.DbMigrator.csproj"

COPY ["src/", "src/"]
RUN dotnet publish "src/Fitliyo.DbMigrator/Fitliyo.DbMigrator.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fitliyo.DbMigrator.dll"]
