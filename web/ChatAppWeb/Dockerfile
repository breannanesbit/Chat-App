# Stage 1: Build the ChatAppWeb Blazor WebAssembly application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ChatAppWeb/*.csproj ./
RUN dotnet restore

COPY ChatAppWeb/. ./
RUN dotnet publish -c Release -o /app

# Stage 2: Create the runtime image
FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

COPY --from=build /app/wwwroot .



# Expose the default port for Nginx
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
