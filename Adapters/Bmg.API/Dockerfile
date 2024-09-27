# Etapa 1: Base para execução da aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa 2: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia e restaura as dependências dos projetos
COPY ["Adapters/Bmg.API/Bmg.API.csproj", "Adapters/Bmg.API/"]
COPY ["Adapters/Bmg.Repository/Bmg.Repository.csproj", "Adapters/Bmg.Repository/"]
COPY ["Application/Bmg.Application/Bmg.Application.csproj", "Application/Bmg.Application/"]
COPY ["BuildingBlocks/Bmg.BuildingBlocks/Bmg.BuildingBlocks.csproj", "BuildingBlocks/Bmg.BuildingBlocks/"]
COPY ["Domain/Bmg.Domain/Bmg.Domain.csproj", "Domain/Bmg.Domain/"]

# Restaura todas as dependências
RUN dotnet restore "./Adapters/Bmg.API/Bmg.API.csproj"

# Copia todo o código-fonte
COPY . .

# Define o diretório de trabalho para o projeto API e faz o build
WORKDIR "/src/Adapters/Bmg.API"
RUN dotnet build "./Bmg.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa 3: Publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bmg.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa 4: Finalizar a imagem
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bmg.API.dll"]
