FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# копируем только csproj
COPY testApp.WebApi/testApp.WebApi.csproj testApp.WebApi/
COPY testApp.Data/testApp.Data.csproj testApp.Data/

# restore для WebApi проекта
RUN dotnet restore testApp.WebApi/testApp.WebApi.csproj

# копируем остальной код
COPY . .

# публикуем WebApi
RUN dotnet publish testApp.WebApi/testApp.WebApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "testApp.WebApi.dll"]
