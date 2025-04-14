# Use the official .NET 8.0 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /out

# Use the official .NET 8.0 runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build /out ./

# Expose the port the application runs on
EXPOSE 5052

# Set the entry point for the container
ENTRYPOINT ["dotnet", "orderflow-security.dll"]