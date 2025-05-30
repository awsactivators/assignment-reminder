# Use Alpine for a smaller image
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /src
COPY AssignmentReminder/*.csproj ./AssignmentReminder/

# Install dependencies for Alpine (if needed)
RUN apk add --no-cache icu-libs bash

# Clear NuGet cache before restore
RUN dotnet nuget locals all --clear

# Restore dependencies
RUN dotnet restore --disable-parallel --verbosity minimal AssignmentReminder/AssignmentReminder.csproj

# Copy remaining files
COPY AssignmentReminder/. ./AssignmentReminder/
WORKDIR /src/AssignmentReminder

# Build and publish
RUN dotnet build -c Release 
RUN dotnet publish -c Release -o /app --no-build

# Use Alpine runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app .

# Expose port
EXPOSE 8088

CMD ["dotnet", "AssignmentReminder.dll"]
