#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Intellipix/Intellipix.csproj", "Intellipix/"]
RUN dotnet restore "Intellipix/Intellipix.csproj"
COPY . .
WORKDIR "/src/Intellipix"
RUN dotnet build "Intellipix.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Intellipix.csproj" -c Release -o /app/publish /p:UseAppHost=false

RUN chmod 777 "wwwroot"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Intellipix.dll"]