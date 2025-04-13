class Config:
    IP_ADDRESS = "192.168.25.61"
    PORT = 443
    SSL_CERT = "certs/server.crt"
    SSL_KEY = "certs/server.key"
    ENCRYPTION_KEY = None  # Set dynamically or use secure env var
