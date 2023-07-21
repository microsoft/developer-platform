FROM mcr.microsoft.com/dotnet/aspnet:7.0

ARG IMAGE_VERSION

# Metadata as defined at http://label-schema.org
ARG BUILD_DATE

ENV DEVELOPER_API_IMAGE_VERSION=$IMAGE_VERSION

LABEL maintainer="Microsoft" \
    org.opencontainers.image.created=$BUILD_DATE \
    org.opencontainers.image.url="https://github.com/microsoft/developer-platform" \
    org.opencontainers.image.source="https://github.com/microsoft/developer-platform" \
    org.opencontainers.image.version=$IMAGE_VERSION \
    org.opencontainers.image.vendor="Microsoft" \
    org.opencontainers.image.title="Microsoft Developer Platform API" \
    org.opencontainers.image.description="The Microsoft Developer Platform API." \
    org.opencontainers.image.documentation="https://github.com/microsoft/developer-platform/tree/main/src/Microsoft.Developer.Api" \
    org.opencontainers.image.licenses="MIT" \
    org.label-schema.schema-version="1.0" \
    org.label-schema.vendor="Microsoft" \
    org.label-schema.name="Microsoft Developer Platform API" \
    org.label-schema.version=$IMAGE_VERSION \
    org.label-schema.license="MIT" \
    org.label-schema.description="The Microsoft Developer Platform API." \
    org.label-schema.url="https://github.com/microsoft/developer-platform" \
    org.label-schema.usage="https://github.com/microsoft/developer-platform/tree/main/src/Microsoft.Developer.Api" \
    org.label-schema.build-date=$BUILD_DATE \
    org.label-schema.vcs-url="https://github.com/microsoft/developer-platform.git" \
    org.label-schema.docker.cmd="docker run ghcr.io/microsoft/developer-platform/api:$IMAGE_VERSION"


ENV PORT 8080
EXPOSE 8080

RUN mkdir -p /home/site/wwwroot
COPY publish /home/site/wwwroot

ENV ASPNETCORE_URLS "http://*:${PORT}"

WORKDIR /home/site/wwwroot

ENTRYPOINT ["dotnet", "/home/site/wwwroot/Microsoft.Developer.Api.dll"]