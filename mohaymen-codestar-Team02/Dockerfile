# <Build>
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o output
# </Build>

# <Run>
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime

LABEL org.opencontainers.image.source="https://github.com/Star-Academy/Summer1403-Project-Group02-Backend/"

USER app

WORKDIR /app

COPY --chown=app:app --from=build /app/output .

ENTRYPOINT ["dotnet", "mohaymen-codestar-Team02.dll"]

EXPOSE 8080

# </Run>
