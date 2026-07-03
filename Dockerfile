# 使用微軟官方的 .NET 執行環境映像檔
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 使用 SDK 映像檔來編譯程式碼
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BulletinBoard.csproj", "."]
RUN dotnet restore "./BulletinBoard.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BulletinBoard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BulletinBoard.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 最後將編譯好的檔案打包
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BulletinBoard.dll"]