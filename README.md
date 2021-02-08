# payment-gateway

Using https://github.com/Huachao/vscode-restclient
## Run PaymentAPI
`docker build --pull -t paymentapi ./PaymentAPI`

`docker run --rm -it -p 8000:80 paymentapi`


## Run PaymentAPI + MongoDB
`docker-compose up --build --detach --force-recreate`

|username:password|Base64| 
|----------:|:-------------:|
|Apple_user:abc1234|QXBwbGVfdXNlcjphYmMxMjM0 |
|Amazon_user:abc1234|QW1hem9uX3VzZXI6YWJjMTIzNA== |
|Farfetch_user:abc1234| RmFyZmV0Y2hfdXNlcjphYmMxMjM0|
|TransferWise_user:abc1234|VHJhbnNmZXJXaXNlX3VzZXI6YWJjMTIzNA==|
