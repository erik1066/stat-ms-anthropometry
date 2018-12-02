# Build stage
FROM microsoft/dotnet:2.1.500-sdk-alpine as publish

RUN apk update && apk upgrade --no-cache

COPY src /src
WORKDIR /src

RUN dotnet publish -c Release

# Run stage
FROM microsoft/dotnet:2.1.6-aspnetcore-runtime-alpine as run

RUN apk update && apk upgrade --no-cache

EXPOSE 9093/tcp
ENV ASPNETCORE_URLS http://*:9093

ARG ASPNETCORE_ENVIRONMENT
ARG APP_NAME

ENV ASPNETCORE_ENVIRONMENT ${ASPNETCORE_ENVIRONMENT}
ENV APP_NAME ${APP_NAME}

COPY --from=publish /src/bin/Release/netcoreapp2.1/publish /app
WORKDIR /app

# pull latest
RUN apk update && apk upgrade --no-cache

# don't run as root user
RUN chown 1001:0 /app/Foundation.AnthStat.WebUI.dll
RUN chmod g+rwx /app/Foundation.AnthStat.WebUI.dll
USER 1001

ENTRYPOINT [ "dotnet", "Foundation.AnthStat.WebUI.dll" ]

