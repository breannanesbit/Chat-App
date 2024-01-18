#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ChatAppAPI/*.csproj ./
RUN dotnet restore

COPY ChatAppAPI/. ./
RUN dotnet publish -c Release -o /app

# Stage 2: Create the runtime image
FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

COPY --from=build /app/wwwroot .



# Expose the default port for Nginx
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
