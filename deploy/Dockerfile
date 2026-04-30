FROM mcr.microsoft.com/powershell:7.4-alpine-3.19 AS build
WORKDIR /workspace
COPY . .
RUN pwsh -File ./deploy/build-pages.ps1 -OutputDir /out/public

FROM nginx:1.27-alpine
COPY deploy/nginx/default.conf /etc/nginx/conf.d/default.conf
COPY --from=build /out/public/ /usr/share/nginx/html/
