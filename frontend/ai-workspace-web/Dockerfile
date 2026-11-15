FROM node:24-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

RUN npm run build

RUN mkdir -p /app/site \
    && if [ -d /app/dist/ai-workspace-web/browser ]; then \
        cp -a /app/dist/ai-workspace-web/browser/. /app/site/; \
    else \
        cp -a /app/dist/ai-workspace-web/. /app/site/; \
    fi

FROM nginxinc/nginx-unprivileged:alpine AS runtime

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/site/ /usr/share/nginx/html/

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]