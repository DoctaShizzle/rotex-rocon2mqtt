# Multi-stage Dockerfile for RoconMqtt (.NET 10)
# Build stage: Publishes the application using the RaspberryPi-ARM64 profile
# Runtime stage: Runs the self-contained executable on a minimal runtime-deps image

# ===========================
# Build Stage
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (for better layer caching)
COPY RoconMqtt.slnx ./
COPY src/RoconMqtt/RoconMqtt.csproj ./src/RoconMqtt/
COPY tests/RoconMqtt.Tests/RoconMqtt.Tests.csproj ./tests/RoconMqtt.Tests/

# Restore dependencies
RUN dotnet restore src/RoconMqtt/RoconMqtt.csproj

# Copy the rest of the application source
COPY src/RoconMqtt/ ./src/RoconMqtt/

# Publish using the RaspberryPi-ARM64 profile (self-contained, trimmed)
RUN dotnet publish src/RoconMqtt/RoconMqtt.csproj \
    -c Release \
    -p:PublishProfile=RaspberryPi-ARM64 \
    -o /app/publish

# ===========================
# Runtime Stage
# ===========================
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS runtime

# Install required runtime dependencies
# - libgcc, libstdc++: for native CAN socket support
# - icu-libs: for internationalization support
RUN apk add --no-cache libgcc libstdc++ icu-libs

WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .

# Make the RoconMqtt binary executable
RUN chmod +x /app/RoconMqtt

# Expose Kestrel HTTP port
EXPOSE 5000

# Set environment variables
# ASPNETCORE_URLS is overridden by appsettings or environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["/app/RoconMqtt"]
