# # Use the official .NET SDK as the base image for building and running
# FROM mcr.microsoft.com/dotnet/sdk:8.0

# # Set the working directory in the container
# WORKDIR /app

# # Copy everything from the current directory into the container
# COPY . .

# # Build the application
# RUN dotnet build -c Release

# # Expose the port that your application listens on
# EXPOSE 8088

# # Run the application
# CMD ["dotnet", "run", "--urls", "http://0.0.0.0:8088"]




# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project file and restore the dependencies
COPY AssignmentReminder.csproj ./
# RUN dotnet restore

# Copy all the other source files to the container
COPY . ./

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime container
WORKDIR /app



# Step 7: Set the ownership and permissions for the static files (optional but recommended)
# # Assuming static files are in the 'dist' directory
#RUN chown -R www-data:www-data /app/wwwroot && \
#    chmod -R 755 /app/wwwroot

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose port  for web access
EXPOSE 5005

# Run the application when the container starts
CMD ["dotnet", "run", "--urls", "http://0.0.0.0:5005"]