FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/IronPrint.Api/IronPrint.Api.csproj", "src/IronPrint.Api/"]
COPY ["src/IronPrint.Application/IronPrint.Application.csproj", "src/IronPrint.Application/"]
COPY ["src/IronPrint.Domain/IronPrint.Domain.csproj", "src/IronPrint.Domain/"]
COPY ["src/IronPrint.Infrastructure/IronPrint.Infrastructure.csproj", "src/IronPrint.Infrastructure/"]
RUN dotnet restore "src/IronPrint.Api/IronPrint.Api.csproj"

COPY . .
RUN dotnet publish "src/IronPrint.Api/IronPrint.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN groupadd --system appgroup && useradd --system --gid appgroup appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "IronPrint.Api.dll"]
