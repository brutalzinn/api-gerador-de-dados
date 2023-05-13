#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
ENV TZ="America/Sao_Paulo"


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["assets/arial.ttf", "/usr/local/share/fonts/"]
RUN dnf install -y fontconfig && fc-cache -f -v
COPY ["GeradorDeDados.csproj", "."]
RUN dotnet restore "./GeradorDeDados.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "GeradorDeDados.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeradorDeDados.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeradorDeDados.dll"]