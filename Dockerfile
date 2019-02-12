# Build stage
FROM microsoft/dotnet:2.2.104-sdk-alpine3.8 as build

ENV DOTNET_CLI_TELEMETRY_OPTOUT true

COPY src /src
WORKDIR /src

RUN dotnet publish -c Release

# Run stage
FROM microsoft/dotnet:2.2.2-aspnetcore-runtime-alpine3.8 as run

RUN apk update && apk upgrade --no-cache

ARG ANTHRO_PORT
ARG ASPNETCORE_ENVIRONMENT
ARG APP_NAME

ENV ANTHRO_PORT ${ANTHRO_PORT}
ENV ASPNETCORE_ENVIRONMENT ${ASPNETCORE_ENVIRONMENT}
ENV APP_NAME ${APP_NAME}

EXPOSE ${ANTHRO_PORT}/tcp
ENV ASPNETCORE_URLS http://*:${ANTHRO_PORT}

COPY --from=build /src/bin/Release/netcoreapp2.2/publish /app
WORKDIR /app

# don't run as root user
RUN chown 1001:0 /app/Foundation.AnthStat.WebUI.dll
RUN chmod g+rwx /app/Foundation.AnthStat.WebUI.dll
USER 1001

ENTRYPOINT [ "dotnet", "Foundation.AnthStat.WebUI.dll" ]

