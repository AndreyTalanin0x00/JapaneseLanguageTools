# See https://aka.ms/customizecontainer to learn how to customize your debug container
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

RUN apt-get update
RUN apt-get install -y ca-certificates curl gnupg
RUN apt-get install -y build-essential libjpeg-dev libpng-dev libxi6 libgl1-mesa-glx

RUN mkdir -p /etc/apt/keyrings
RUN curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg
RUN echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list
RUN apt-get update
RUN apt-get install -y nodejs

WORKDIR "/source"

COPY ["JapaneseLanguageTools.Contracts/JapaneseLanguageTools.Contracts.csproj", "JapaneseLanguageTools.Contracts/"]
COPY ["JapaneseLanguageTools.Contracts.Json/JapaneseLanguageTools.Contracts.Json.csproj", "JapaneseLanguageTools.Contracts.Json/"]
COPY ["JapaneseLanguageTools.Contracts.Xml/JapaneseLanguageTools.Contracts.Xml.csproj", "JapaneseLanguageTools.Contracts.Xml/"]
COPY ["JapaneseLanguageTools.Core/JapaneseLanguageTools.Core.csproj", "JapaneseLanguageTools.Core/"]
COPY ["JapaneseLanguageTools.Data/JapaneseLanguageTools.Data.csproj", "JapaneseLanguageTools.Data/"]
COPY ["JapaneseLanguageTools.Data.Sqlite/JapaneseLanguageTools.Data.Sqlite.csproj", "JapaneseLanguageTools.Data.Sqlite/"]
COPY ["JapaneseLanguageTools.Data.SqlServer/JapaneseLanguageTools.Data.SqlServer.csproj", "JapaneseLanguageTools.Data.SqlServer/"]
COPY ["JapaneseLanguageTools/JapaneseLanguageTools.csproj", "JapaneseLanguageTools/"]

RUN dotnet restore "JapaneseLanguageTools/JapaneseLanguageTools.csproj"

COPY "." "."

RUN dotnet build "JapaneseLanguageTools/JapaneseLanguageTools.csproj" -o "/application/build" -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "JapaneseLanguageTools/JapaneseLanguageTools.csproj" -o "/application/publish" -c $BUILD_CONFIGURATION --no-restore /p:UseAppHost=false

FROM base AS final

WORKDIR "/application"

COPY --from=publish "/application/publish" "."

ENTRYPOINT ["dotnet", "JapaneseLanguageTools.dll"]
