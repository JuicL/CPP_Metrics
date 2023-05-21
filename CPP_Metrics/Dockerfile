#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

RUN apt-get update && apt-get -y install g++


COPY ["test.cpp", "/testapp/"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CPP_Metrics/CPP_Metrics.csproj", "CPP_Metrics/"]
COPY ["ANTLR4Tools/ANTLR4Tools.csproj", "ANTLR4Tools/"]
RUN dotnet restore "CPP_Metrics/CPP_Metrics.csproj"
COPY . .
WORKDIR "/src/CPP_Metrics"
RUN dotnet build "CPP_Metrics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CPP_Metrics.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CPP_Metrics.dll"]


