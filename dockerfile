# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app
# Copiar archivos de proyecto y restaurar dependencias
COPY Chaos/Chaos.Api.csproj ./Chaos/
COPY Chaos.Infraestructure/Chaos.Infraestructure.csproj ./Chaos.Infraestructure/
COPY Chaos.Shared/Chaos.Shared.csproj ./Chaos.Shared/
RUN dotnet restore Chaos/Chaos.Api.csproj
# Copiar todo lo demás y compilar
COPY . ./
RUN dotnet publish Chaos/Chaos.Api.csproj -c Release -o out
# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Chaos.Api.dll"]
