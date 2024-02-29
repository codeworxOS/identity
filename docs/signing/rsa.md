# Generate RSA signing certificate

```shell
openssl genrsa -out privatekey.pem 3072

openssl rsa -in privatekey.pem -pubout -out publickey.pem

openssl req -new -x509 -key privatekey.pem -out cert.pem -days 1080

openssl pkcs12 -export -inkey privatekey.pem -in cert.pem -out cert.pfx
```