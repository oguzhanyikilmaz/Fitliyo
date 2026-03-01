FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build
COPY . .
RUN mkdir -p src/Fitliyo.EntityFrameworkCore/bin/Debug \
    && mkdir -p src/Fitliyo.EntityFrameworkCore/bin/Release \
    && dotnet publish src/Fitliyo.DbMigrator/Fitliyo.DbMigrator.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fitliyo.DbMigrator.dll"]