# Stage 1: Build the ChatAppWeb Blazor WebAssembly application
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ChatAppWeb/*.csproj ./
COPY Shared/*.csproj ./Shared/
RUN dotnet restore

# Copy the source code and build the application
COPY ChatAppWeb/. ./ChatAppWeb/
COPY Shared/. ./Shared/
RUN dotnet publish -c Release -o /app

# Stage 2: Create the runtime image
FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

# Copy the published output from the build stage
COPY --from=build /app/wwwroot .

# Expose the default port for Nginx
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
