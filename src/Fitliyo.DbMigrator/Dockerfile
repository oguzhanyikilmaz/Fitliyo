FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

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
    -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fitliyo.DbMigrator.dll"]
