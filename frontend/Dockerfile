FROM node:22-alpine AS build

WORKDIR /app

COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci

COPY frontend/ .
RUN npx ng build --configuration production

# Runtime stage
FROM nginx:alpine AS runtime

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy nginx template
COPY ops/nginx.conf /etc/nginx/templates/default.conf.template

# Only substitute the DOMAIN variable (avoids replacing nginx $host, $uri, etc.)
ENV NGINX_ENVSUBST_FILTER=DOMAIN

# Copy built Angular app
COPY --from=build /app/dist/angularnetbase.client/browser/ /usr/share/nginx/html

EXPOSE 80 443
