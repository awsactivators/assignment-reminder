# Use the official .NET SDK as the base image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy only the project file first for better caching
COPY AssignmentReminder/*.csproj ./AssignmentReminder/

# Restore dependencies before copying the entire project
RUN dotnet restore AssignmentReminder/AssignmentReminder.csproj

# Copy the rest of the application files
COPY AssignmentReminder/. ./AssignmentReminder/
WORKDIR /src/AssignmentReminder

# Build the application
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
