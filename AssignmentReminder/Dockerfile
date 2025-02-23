# Use official .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy only the project file first for better caching
COPY AssignmentReminder/*.csproj ./AssignmentReminder/

# Ensure NuGet cache is cleared before restoring dependencies
RUN dotnet nuget locals all --clear

# Restore dependencies explicitly
RUN dotnet restore --disable-parallel --verbosity minimal AssignmentReminder/AssignmentReminder.csproj

# Copy the rest of the application files after restore is complete
COPY AssignmentReminder/. ./AssignmentReminder/
WORKDIR /src/AssignmentReminder

# Build the application (without restoring)
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish -c Release -o /app --no-build

# Use a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Expose the port your application listens on
EXPOSE 8088

# Run the application
CMD ["dotnet", "AssignmentReminder.dll"]
