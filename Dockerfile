# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore as distinct layers to leverage Docker cache
COPY *.sln .
COPY *.csproj .
RUN rm -rf obj
RUN dotnet restore "./AnastasiiaPortfolio.csproj"

# Copy everything else and build app
COPY . .
WORKDIR /source
RUN dotnet clean "./AnastasiiaPortfolio.csproj" -c Release
RUN dotnet publish "./AnastasiiaPortfolio.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Serve the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app runs on (default is 8080 for ASP.NET Core in containers)
EXPOSE 8080

# Set the entrypoint
ENTRYPOINT ["dotnet", "AnastasiiaPortfolio.dll"]

# Optional: Add environment variable for ASP.NET Core URLs if needed
# ENV ASPNETCORE_URLS=http://+:$PORT
ENV ASPNETCORE_URLS=http://+:8080 