## Stage 1: Build the Blazor WebAssembly app
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#
#COPY ["ChatAppWeb/ChatAppWeb.csproj", "ChatAppWeb/"]
#RUN dotnet restore "ChatAppWeb/ChatAppWeb.csproj"
#
#COPY . .
#WORKDIR "/src/ChatAppWeb"
#RUN dotnet build "ChatAppWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## Stage 2: Publish the Blazor WebAssembly app
#FROM build AS publish
#RUN dotnet publish "ChatAppWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM nginx:alpine
#WORKDIR /user/share/nginx/html
#COPY --from=publish /app/publish/wwwroot .
#COPY ChatAppWeb/nginx.conf /etc/nginx/nginx.conf


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChatAppWeb/ChatAppWeb.csproj", "ChatAppWeb/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "./ChatAppWeb/./ChatAppWeb.csproj"
COPY . .
WORKDIR "/src/ChatAppWeb"
RUN dotnet build "./ChatAppWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChatAppWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatAppWeb.dll"]

# Stage 3: Final image
#FROM nginx:alpine AS final
#WORKDIR /usr/share/nginx/html
#
## Copy the published Blazor WebAssembly app
#COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html
#
## Copy the nginx configuration
#COPY ["ChatAppWeb/nginx.conf", "/etc/nginx/nginx.conf"]
#
#EXPOSE 80
#ENTRYPOINT ["nginx", "-g", "daemon off;"]



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