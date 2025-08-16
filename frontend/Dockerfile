# 1. Build stage
FROM node:20 AS build
WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm install

# Copy all source files
COPY . .

# Build Angular app
RUN npm run build --configuration=production --output-path=dist/IotRegistryFrontend

# 2. Production stage with Nginx
FROM nginx:alpine

# Remove default Nginx HTML
RUN rm -rf /usr/share/nginx/html/*

# Copy Angular build
COPY --from=build /app/dist/IotRegistryFrontend/browser/ /usr/share/nginx/html/

# Copy custom Nginx config
COPY default.conf /etc/nginx/conf.d/default.conf

# Expose port
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
