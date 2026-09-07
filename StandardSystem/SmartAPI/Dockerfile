# Onfinity Smart API — container image. Build context = the SmartAPI/ directory.
#   docker build -t onfinity-smartapi:latest .

# ---- build ----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# restore first (layer-cached on unchanged csproj)
COPY src/OnfinitySmartApi/OnfinitySmartApi.csproj src/OnfinitySmartApi/
RUN dotnet restore src/OnfinitySmartApi/OnfinitySmartApi.csproj
# source + manifests (csproj copies ../../manifests into the output)
COPY src/ src/
COPY manifests/ manifests/
RUN dotnet publish src/OnfinitySmartApi/OnfinitySmartApi.csproj -c Release -o /app /p:UseAppHost=false

# ---- runtime --------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app .
# guarantee manifests are present regardless of csproj glob behavior on Linux
COPY manifests/ ./manifests/

# Kestrel listens on 8080 inside the container (map to a host port in compose).
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# run as the image's non-root user
USER $APP_UID
ENTRYPOINT ["dotnet", "OnfinitySmartApi.dll"]
