FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

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
