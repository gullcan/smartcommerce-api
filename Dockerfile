# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution + csproj files for restore caching
COPY SmartCommerce.sln ./
COPY src/SmartCommerce.Domain/SmartCommerce.Domain.csproj src/SmartCommerce.Domain/
COPY src/SmartCommerce.Application/SmartCommerce.Application.csproj src/SmartCommerce.Application/
COPY src/SmartCommerce.Infrastructure/SmartCommerce.Infrastructure.csproj src/SmartCommerce.Infrastructure/
COPY src/SmartCommerce.Api/SmartCommerce.Api.csproj src/SmartCommerce.Api/

RUN dotnet restore src/SmartCommerce.Api/SmartCommerce.Api.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish src/SmartCommerce.Api/SmartCommerce.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SmartCommerce.Api.dll"]
