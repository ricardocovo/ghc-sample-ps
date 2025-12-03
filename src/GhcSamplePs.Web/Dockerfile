# Multi-stage build for .NET 10 Blazor Server
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["GhcSamplePs.sln", "./"]
COPY ["src/GhcSamplePs.Web/GhcSamplePs.Web.csproj", "src/GhcSamplePs.Web/"]
COPY ["src/GhcSamplePs.Core/GhcSamplePs.Core.csproj", "src/GhcSamplePs.Core/"]

# Restore dependencies
RUN dotnet restore "src/GhcSamplePs.Web/GhcSamplePs.Web.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/src/GhcSamplePs.Web"
RUN dotnet build "GhcSamplePs.Web.csproj" -c Release -o /app/build
RUN dotnet publish "GhcSamplePs.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app

# Copy published app
COPY --from=build /app/publish .

# Switch to non-root user
USER appuser

# Expose port 8080 (Container Apps standard)
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GhcSamplePs.Web.dll"]
