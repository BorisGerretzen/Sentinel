FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sentinel/Sentinel.csproj", "Sentinel/"]
RUN dotnet restore "Sentinel/Sentinel.csproj"
COPY . .
WORKDIR "/src/Sentinel"
RUN dotnet build "Sentinel.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sentinel.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sentinel.dll"]
