FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
COPY ./UnitTestWebExample.Test/*.csproj ./UnitTestWebExample.Test/
COPY ./UnitTestWebExample.Web/*.csproj ./UnitTestWebExample.Web/
COPY *.sln .
RUN dotnet restore
COPY . .
RUN dotnet test ./UnitTestWebExample.Test/*.csproj
RUN dotnet publish ./UnitTestWebExample.Web -o /publish/
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .
ENV ASPNETCORE_URL="http://*:5000"
ENTRYPOINT [ "dotnet","UnitTestWebExample.Web.dll" ]