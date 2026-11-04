# ==========================================
# ETAPA 1: Compilacion con .NET SDK 8.0
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solucion y proyecto para restaurar paquetes NuGet
COPY ["syswb_Taller.sln", "./"]
COPY ["syswb_Taller/syswb_Taller.csproj", "syswb_Taller/"]
RUN dotnet restore "syswb_Taller/syswb_Taller.csproj"

# Copiar el codigo fuente completo
COPY . .
WORKDIR "/src/syswb_Taller"

# Compilar en modo Release optimizado
RUN dotnet publish "syswb_Taller.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# ETAPA 2: Entorno de Ejecucion Ligero (Runtime)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Ajustar OpenSSL SECLEVEL a 1 para permitir conexion TLS con SQL Server
USER root
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf || true && \
    sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf || true && \
    sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf || true

# Puerto estandar que utilizan .NET 8 y Railway
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Copiar binarios compilados desde la etapa anterior
COPY --from=build /app/publish .

# Comando de inicio del sistema
ENTRYPOINT ["dotnet", "syswb_Taller.dll"]
