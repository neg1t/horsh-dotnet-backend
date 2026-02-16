FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# копируем solution
COPY *.sln .

# копируем проекты
COPY testApp.WebApi/testApp.WebApi.csproj testApp.WebApi/
COPY testApp.Data/testApp.Data.csproj testApp.Data/

# restore зависимостей
RUN dotnet restore

# копируем всё остальное
COPY . .

# публикуем именно WebApi проект
RUN dotnet publish testApp.WebApi/testApp.WebApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "testApp.WebApi.dll"]
