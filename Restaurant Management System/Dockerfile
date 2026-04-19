# Use the .NET Framework SDK to build the app
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /app

# Copy the project and build it
COPY . .
RUN msbuild /p:Configuration=Release

# Use the Runtime image for the final container
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8
WORKDIR /app

# Copy the compiled .exe and files from the build stage
COPY --from=build /app/bin/Release/ .

# Run your application
ENTRYPOINT ["Restaurant Management System.exe"]