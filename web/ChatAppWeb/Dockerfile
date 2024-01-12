FROM node:20 as build 

WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . . 
RUN npm run build
RUN ls 


FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /app/build /usr/share/nginx/html
COPY ./ChatAppWeb/client-default.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]