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
COPY ["ECommerceApp.Shared/ECommerceApp.Shared.csproj", "ECommerceApp.Shared/"]

# Restore dependencies
RUN dotnet restore "ECommerceApp.Api/ECommerceApp.Api.csproj"
RUN dotnet restore "ECommerceApp.Web/ECommerceApp.Web.csproj"

# Copy the rest of the code
COPY . .

# Build and publish API
WORKDIR "/src/ECommerceApp.Api"
RUN dotnet build "ECommerceApp.Api.csproj" -c Release -o /app/build/api
RUN dotnet publish "ECommerceApp.Api.csproj" -c Release -o /app/publish/api /p:UseAppHost=false

# Build and publish Web
WORKDIR "/src/ECommerceApp.Web"
RUN dotnet build "ECommerceApp.Web.csproj" -c Release -o /app/build/web
RUN dotnet publish "ECommerceApp.Web.csproj" -c Release -o /app/publish/web /p:UseAppHost=false

# Development stage for API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS api-dev
WORKDIR /src
COPY . .
WORKDIR "/src/ECommerceApp.Api"
ENTRYPOINT ["dotnet", "watch", "run", "--no-restore", "--urls", "http://+:80"]

# Development stage for Web
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS web-dev
WORKDIR /src
COPY . .
WORKDIR "/src/ECommerceApp.Web"
ENTRYPOINT ["dotnet", "watch", "run", "--no-restore", "--urls", "http://+:80"]

# Final stage for API
FROM base AS api
WORKDIR /app
COPY --from=build /app/publish/api ./
ENTRYPOINT ["dotnet", "ECommerceApp.Api.dll"]

# Final stage for Web
FROM base AS web
WORKDIR /app
COPY --from=build /app/publish/web ./
ENTRYPOINT ["dotnet", "ECommerceApp.Web.dll"] 