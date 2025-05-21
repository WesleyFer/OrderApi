# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root
RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*

USER $APP_UID
WORKDIR /app
EXPOSE 7127

ENV ASPNETCORE_URLS=http://+:7127

ENV TZ=America/Sao_Paulo

ENV LANG=pt_BR.UTF-8
ENV LANGUAGE=${LANG}
ENV LC_ALL=${LANG}

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Order.Api/Order.Api.csproj", "Order.Api/"]
COPY ["Order.Aplicacao/Order.Aplicacao.csproj", "Order.Aplicacao/"]
COPY ["Order.Dominio/Order.Dominio.csproj", "Order.Dominio/"]
COPY ["Order.Infra/Order.Infra.csproj", "Order.Infra/"]
RUN dotnet restore "./Order.Api/Order.Api.csproj"
COPY . .
WORKDIR "/src/Order.Api"
RUN dotnet build "./Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY entrypoint.sh .

USER root
RUN chmod +x entrypoint.sh
USER $APP_UID

ENTRYPOINT ["./entrypoint.sh"]