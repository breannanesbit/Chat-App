# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /app

# Copy the project files to the container
COPY . .

# Build the Blazor WebAssembly project
RUN dotnet publish -c Release -o out

# Use a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

WORKDIR /app

# Copy the published output from the build image
COPY --from=build /app/out .

# Expose the port your application will run on
EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "ChatAppWeb.dll"]

#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /app
#COPY . .
#
#COPY ["ChatAppWeb/ChatAppWeb.csproj", "ChatAppWeb/"]
#COPY ["Shared/Shared.csproj", "Shared/"]
#RUN dotnet restore "./ChatAppWeb/ChatAppWeb.csproj"
#
#RUN dotnet publish -c Release -o out
#
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#
#WORKDIR /app
#
## Copy the project files and restore dependencies
#
#
## Copy the source code and build the application
#COPY ChatAppWeb/. ./ChatAppWeb/
#COPY Shared/. ./Shared/
#RUN dotnet publish -c Release -o /app
#
#
## Copy the published output from the build image
#COPY --from=build /app/out .
#
## Expose the port your application will run on
#EXPOSE 80
#
## Set the entry point for the application
#ENTRYPOINT ["dotnet", "ChatAppWeb.dll"]