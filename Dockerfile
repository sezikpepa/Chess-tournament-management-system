#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8000
EXPOSE 8081

EXPOSE 1433

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChessTournamentManager/ChessTournamentManager.csproj", "ChessTournamentManager/"]
COPY ["ChessTournamentManager.Client/ChessTournamentManager.Client.csproj", "ChessTournamentManager.Client/"]
COPY ["DatabaseCommunicator/DatabaseCommunicator.csproj", "DatabaseCommunicator/"]
COPY ["Shared/ChessTournamentManager.Shared.csproj", "Shared/"]
COPY ["TournamentLibrary/TournamentLibrary.csproj", "TournamentLibrary/"]
RUN dotnet restore "./ChessTournamentManager/ChessTournamentManager.csproj"
COPY . .
WORKDIR "/src/ChessTournamentManager"
RUN dotnet build "./ChessTournamentManager.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChessTournamentManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

COPY init.sql /docker-entrypoint-initdb.d/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChessTournamentManager.dll"]