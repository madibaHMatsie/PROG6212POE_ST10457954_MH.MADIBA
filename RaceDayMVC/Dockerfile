# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RaceDayMVC/RaceDayMVC.csproj", "RaceDayMVC/"]
RUN dotnet restore "RaceDayMVC/RaceDayMVC.csproj"
COPY . .
WORKDIR "/src/RaceDayMVC"
RUN dotnet build "RaceDayMVC.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "RaceDayMVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDayMVC.dll"]