FROM mongo
ARG URI
ENV URI ${URI}
COPY *.json ./
CMD mongoimport --uri ${URI} --db payment-api --collection users --type json --file ./users.json --jsonArray
