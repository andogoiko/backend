FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 4000

ENV ASPNETCORE_URLS=http://+:4000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ProcesoBackend/ProcesoBackend.csproj", "ProcesoBackend/"]
RUN dotnet restore "ProcesoBackend\ProcesoBackend.csproj"
COPY . .
WORKDIR "/src/ProcesoBackend"
RUN dotnet build "ProcesoBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProcesoBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProcesoBackend.dll"]
