FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
ENV URLS="http://*:8080"
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Hbk.Platform/Hbk.Platform.csproj", "Hbk.Platform/"]
RUN dotnet restore "Hbk.Platform/Hbk.Platform.csproj"
COPY . .
WORKDIR "/src/Hbk.Platform"
RUN dotnet build "Hbk.Platform.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Hbk.Platform.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hbk.Platform.dll"]


LABEL org.opencontainers.image.source=https://github.com/mjb8086/ProjectHabakkuk
