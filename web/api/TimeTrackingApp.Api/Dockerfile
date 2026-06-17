ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

COPY TimeTrackingApp.Api/*.csproj TimeTrackingApp.Api/
COPY TimeTrackingApp.BL/*.csproj TimeTrackingApp.BL/
COPY TimeTrackingApp.DAL/*.csproj TimeTrackingApp.DAL/
RUN dotnet restore

COPY . .
RUN dotnet publish TimeTrackingApp.Api/TimeTrackingApp.Api.csproj -c Release -o /app/publish

ARG DOTNET_VERSION
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TimeTrackingApp.Api.dll"]