# Runtime .NET 10 LTS
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# SDK .NET 10 LTS
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем csproj
COPY ShortLinks.csproj ./
RUN dotnet restore

# Копируем весь проект
COPY . ./

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShortLinks.dll"]
