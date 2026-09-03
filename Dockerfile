# Stage 1: Base runtime (Ubuntu Chiseled hardened minimal image)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/ChessTrainer/ChessTrainer.csproj", "src/ChessTrainer/"]
RUN dotnet restore "src/ChessTrainer/ChessTrainer.csproj"

COPY . .
WORKDIR "/src/src/ChessTrainer"
RUN dotnet build "ChessTrainer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChessTrainer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChessTrainer.dll"]
