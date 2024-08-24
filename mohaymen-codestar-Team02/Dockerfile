# <Build>
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet build -c Release
RUN dotnet publish -c Release -o output --no-build
RUN dotnet ef migrations bundle --no-build -o dbmigrate
# </Build>

# <Run>
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

LABEL org.opencontainers.image.source="https://github.com/Star-Academy/Summer1403-Project-Group02-Backend/"

USER app

WORKDIR /app

COPY --chown=app:app --from=build /app/output /app/dbmigrate /app/
COPY --chown=app:app ./entrypoint.sh /app/entrypoint.sh

ENTRYPOINT ["bash", "/app/entrypoint.sh"]

EXPOSE 8080

# </Run>
