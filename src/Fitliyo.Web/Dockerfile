FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build
COPY . .
RUN find src -name "*.csproj" | sed 's|/[^/]*\.csproj||' | xargs -I{} mkdir -p {}/bin/Debug {}/bin/Release \
    && dotnet publish src/Fitliyo.Web/Fitliyo.Web.csproj \
    -c Release \
    -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS="http://+:80"
ENV ASPNETCORE_ENVIRONMENT="Production"

ENTRYPOINT ["dotnet", "Fitliyo.Web.dll"]