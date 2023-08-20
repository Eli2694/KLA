#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UniqueIdsScannerUI/UniqueIdsScannerUI.csproj", "UniqueIdsScannerUI/"]
COPY ["DAL/DAL.csproj", "DAL/"]
COPY ["Model/Model.csproj", "Model/"]
COPY ["Utility_LOG/Utility_LOG.csproj", "Utility_LOG/"]
COPY ["Entity/Entity.csproj", "Entity/"]
COPY ["Repository/Repository.csproj", "Repository/"]

RUN dotnet restore "UniqueIdsScannerUI/UniqueIdsScannerUI.csproj"

COPY . .

WORKDIR "/src/UniqueIdsScannerUI"

RUN dotnet build "UniqueIdsScannerUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UniqueIdsScannerUI.csproj" -c Release -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY InputFiles ./InputFiles
CMD ["tail", "-f", "/dev/null"]
