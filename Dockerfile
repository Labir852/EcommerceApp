# Base stage for both API and Web
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage for shared projects
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first
COPY ["ECommerceApp.Api/ECommerceApp.Api.csproj", "ECommerceApp.Api/"]
COPY ["ECommerceApp.Web/ECommerceApp.Web.csproj", "ECommerceApp.Web/"]

# Restore dependencies
RUN dotnet restore "ECommerceApp.Api/ECommerceApp.Api.csproj"
RUN dotnet restore "ECommerceApp.Web/ECommerceApp.Web.csproj"

# Copy the rest of the code
COPY . .

# Build and publish API
WORKDIR "/src/ECommerceApp.Api"
RUN dotnet build "ECommerceApp.Api.csproj" -c Release -o /app/build/api
RUN dotnet publish "ECommerceApp.Api.csproj" -c Release -o /app/publish/api /p:UseAppHost=false /p:PublishDir=/app/publish/api

# Build and publish Web
WORKDIR "/src/ECommerceApp.Web"
RUN dotnet build "ECommerceApp.Web.csproj" -c Release -o /app/build/web
RUN dotnet publish "ECommerceApp.Web.csproj" -c Release -o /app/publish/web /p:UseAppHost=false /p:PublishDir=/app/publish/web

# Final stage for API
FROM base AS api
WORKDIR /app
COPY --from=build /app/publish/api ./
COPY ECommerceApp.Api/appsettings*.json ./
ENTRYPOINT ["dotnet", "ECommerceApp.Api.dll"]

# Final stage for Web
FROM base AS web
WORKDIR /app
COPY --from=build /app/publish/web ./
COPY ECommerceApp.Web/appsettings*.json ./
ENTRYPOINT ["dotnet", "ECommerceApp.Web.dll"] 