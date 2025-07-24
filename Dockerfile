# builder
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/*.csproj ./
COPY src/ ./

RUN dotnet publish -c Release --no-self-contained -o /app/publish

# runner
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "cogitare.dll"]
