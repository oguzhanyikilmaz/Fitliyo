FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# SSL/NuGet sorunlarını environment ile çöz (apt-get gerektirmez)
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
ENV NUGET_CERT_REVOCATION_MODE=offline

COPY ["NuGet.Config", "."]
COPY ["common.props", "."]
COPY ["Fitliyo.slnx", "."]

COPY ["src/Fitliyo.Domain.Shared/Fitliyo.Domain.Shared.csproj", "src/Fitliyo.Domain.Shared/"]
COPY ["src/Fitliyo.Domain/Fitliyo.Domain.csproj", "src/Fitliyo.Domain/"]
COPY ["src/Fitliyo.Application.Contracts/Fitliyo.Application.Contracts.csproj", "src/Fitliyo.Application.Contracts/"]
COPY ["src/Fitliyo.Application/Fitliyo.Application.csproj", "src/Fitliyo.Application/"]
COPY ["src/Fitliyo.EntityFrameworkCore/Fitliyo.EntityFrameworkCore.csproj", "src/Fitliyo.EntityFrameworkCore/"]
COPY ["src/Fitliyo.HttpApi/Fitliyo.HttpApi.csproj", "src/Fitliyo.HttpApi/"]
COPY ["src/Fitliyo.HttpApi.Client/Fitliyo.HttpApi.Client.csproj", "src/Fitliyo.HttpApi.Client/"]
COPY ["src/Fitliyo.Web/Fitliyo.Web.csproj", "src/Fitliyo.Web/"]

RUN dotnet restore "src/Fitliyo.Web/Fitliyo.Web.csproj"

COPY ["src/", "src/"]
RUN dotnet publish "src/Fitliyo.Web/Fitliyo.Web.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS="http://+:80"
ENV ASPNETCORE_ENVIRONMENT="Production"

ENTRYPOINT ["dotnet", "Fitliyo.Web.dll"]
