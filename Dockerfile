FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS installer-env

COPY ./EventStore.Repository /src/EventStore.Repository
COPY ./EventStore.Models /src/EventStore.Models
COPY ./EventStore.Core /src/EventStore.Core
COPY ./EventStore.Functions /src/EventStore.Functions

RUN cd /src/EventStore.Functions && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --configuration Release --output /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet:3.0

ENV AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    AzureWebJobsStorage="UseDevelopmentStorage=true" \
    OPEN_ID_CONNECT_METADATA_ADDRESS="" \
    CLIENT_ID="" \
    DB_CONNECTION_STRING=""

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]
