# Use the official .NET SDK as the base image for building and running
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the project file and restore dependencies
COPY AssignmentReminder/*.csproj ./AssignmentReminder/
RUN dotnet restore AssignmentReminder/AssignmentReminder.csproj

# Copy the rest of the application files and move to the project directory
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
