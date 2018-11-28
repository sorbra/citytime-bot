FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["mybot.csproj", "./"]
RUN dotnet restore "./mybot.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "mybot.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "mybot.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "mybot.dll"]
