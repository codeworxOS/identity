# Generate ECC signing certificate

```shell
openssl ecparam -name secp521r1 -genkey -noout -out privatekey.pem

openssl ec -in privatekey.pem -pubout -out publickey.pem

openssl req -new -x509 -key privatekey.pem -out cert.pem -days 1080 -sha512

openssl pkcs12 -export -inkey privatekey.pem -in cert.pem -out cert.pfx
```