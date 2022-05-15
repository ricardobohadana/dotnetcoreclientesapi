#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GestaoDeClientesApi.Services/GestaoDeClientesApi.Services.csproj", "GestaoDeClientesApi.Services/"]
COPY ["GestaoDeClientesApi.Repository/GestaoDeClientesApi.Infra.Data.csproj", "GestaoDeClientesApi.Repository/"]
COPY ["GestaoDeClientesApi.Domain/GestaoDeClientesApi.Domain.csproj", "GestaoDeClientesApi.Domain/"]
RUN dotnet restore "GestaoDeClientesApi.Services/GestaoDeClientesApi.Services.csproj"
COPY . .
WORKDIR "/src/GestaoDeClientesApi.Services"
RUN dotnet build "GestaoDeClientesApi.Services.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GestaoDeClientesApi.Services.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestaoDeClientesApi.Services.dll"]