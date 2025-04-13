import socket
import ssl
from .logger import logger
from .encryptor import Encryptor

class RemoteSession:
    def __init__(self, config):
        self.config = config
        self.encryptor = Encryptor(config.ENCRYPTION_KEY)

    def initialize(self):
        logger.info("Initializing remote session over TLS.")
        self.context = ssl.create_default_context(ssl.Purpose.CLIENT_AUTH)
        self.context.load_cert_chain(certfile=self.config.SSL_CERT, keyfile=self.config.SSL_KEY)

    def listen_for_commands(self):
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
            sock.bind((self.config.IP_ADDRESS, self.config.PORT))
            sock.listen(5)
            with self.context.wrap_socket(sock, server_side=True) as ssock:
                logger.info(f"TLS-secured control session listening on {self.config.IP_ADDRESS}:{self.config.PORT}")
                while True:
                    conn, addr = ssock.accept()
                    with conn:
                        encrypted_data = conn.recv(4096)
                        command = self.encryptor.decrypt(encrypted_data).decode()
                        logger.info(f"Received command: {command} from {addr}")
                        # Execute securely
