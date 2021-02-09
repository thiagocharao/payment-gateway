# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /source
# ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT Development
ENV ASPNETCORE_URLS=http://+:5000

# Copy everything and build
COPY . ./
RUN dotnet restore
RUN dotnet publish "PaymentAPI.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "PaymentAPI.dll"]
