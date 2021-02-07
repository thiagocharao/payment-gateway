# payment-gateway


## Run PaymentAPI
`docker build --pull -t paymentapi ./PaymentAPI`

`docker run --rm -it -p 8000:80 paymentapi`


## Run PaymentAPI + MongoDB
`docker-compose up --build --detach --force-recreate`
