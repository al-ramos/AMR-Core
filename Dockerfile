FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/AMR.Core.Shared/AMR.Core.Shared.csproj           src/AMR.Core.Shared/
COPY src/AMR.Core.Domain/AMR.Core.Domain.csproj           src/AMR.Core.Domain/
COPY src/AMR.Core.Application/AMR.Core.Application.csproj src/AMR.Core.Application/
COPY src/AMR.Core.Infrastructure/AMR.Core.Infrastructure.csproj src/AMR.Core.Infrastructure/
COPY src/AMR.Core.API/AMR.Core.API.csproj                 src/AMR.Core.API/

RUN dotnet restore src/AMR.Core.API/AMR.Core.API.csproj

COPY src/ src/
RUN dotnet publish src/AMR.Core.API/AMR.Core.API.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "AMR.Core.API.dll"]
