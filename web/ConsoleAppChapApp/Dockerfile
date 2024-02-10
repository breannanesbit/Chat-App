
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConsoleAppChapApp/ConsoleAppChapApp.csproj", "ConsoleAppChapApp/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "./ConsoleAppChapApp/./ConsoleAppChapApp.csproj"
COPY . .
WORKDIR "/src/ConsoleAppChapApp"
RUN dotnet build "./ConsoleAppChapApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConsoleAppChapApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConsoleAppChapApp.dll"]