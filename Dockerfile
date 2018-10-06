FROM microsoft/dotnet:2.1.403-sdk-alpine as publish
COPY src /src
WORKDIR /src

RUN dotnet publish -c Release

FROM microsoft/dotnet:2.1.5-aspnetcore-runtime-alpine as run
COPY --from=publish /src/bin/Release/netcoreapp2.1/publish /app
WORKDIR /app

EXPOSE 9093/tcp
ENV ASPNETCORE_URLS http://*:9093

ARG ASPNETCORE_ENVIRONMENT
ARG APP_NAME

ENV ASPNETCORE_ENVIRONMENT ${ASPNETCORE_ENVIRONMENT}
ENV APP_NAME ${APP_NAME}

# pull latest
RUN apk update && apk upgrade --no-cache

# don't run as root user
RUN chown 1001:0 /app/Foundation.AnthStat.WebUI.dll
RUN chmod g+rwx /app/Foundation.AnthStat.WebUI.dll
USER 1001

ENTRYPOINT [ "dotnet", "Foundation.AnthStat.WebUI.dll" ]

