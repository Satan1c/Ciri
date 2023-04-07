FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
COPY ["Ciri/bin/Localizations", "root/.net/Localizations"]
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Ciri/Ciri.csproj", "Ciri/"]
COPY ["DataBase/DataBase.csproj", "DataBase/"]
COPY ["Localization/Localization.csproj", "Localization/"]
RUN dotnet restore "Ciri/Ciri.csproj"
COPY . .
WORKDIR "/src/Ciri"
RUN dotnet build "Ciri.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ciri.csproj" --os linux --arch x64 --sc -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./Ciri"]
