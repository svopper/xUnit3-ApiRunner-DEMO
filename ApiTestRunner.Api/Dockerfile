FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiTestRunner.Api/ApiTestRunner.Api.csproj", "ApiTestRunner.Api/"]
COPY ["ApiTestRunner.Tests/ApiTestRunner.Tests.csproj", "ApiTestRunner.Tests/"]
RUN dotnet restore "ApiTestRunner.Api/ApiTestRunner.Api.csproj"
COPY . .
WORKDIR "/src/ApiTestRunner.Api"
RUN dotnet build "./ApiTestRunner.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiTestRunner.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiTestRunner.Api.dll"]

# To build the Docker image, use the following command:
# docker buildx build -t apirunner -f ApiTestRunner.Api/Dockerfile .
# docker run -p 8080:8080 apirunner