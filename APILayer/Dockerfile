FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["APILayer/APILayer.csproj", "APILayer/"]
RUN dotnet restore "APILayer/APILayer.csproj"
COPY . . 
WORKDIR "/src/APILayer"
RUN dotnet build "./APILayer.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "./APILayer.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV MYSQL_HOST=localhost
ENV MYSQL_PASSWORD=user@1234
ENV MYSQL_PORT=3306
ENV MYSQL_DATABASE=ecommerceproductsdatabase
ENV MYSQL_USERID=root
ENTRYPOINT ["dotnet", "APILayer.dll"]
