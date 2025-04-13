from cryptography.fernet import Fernet
from .config import Config
from .logger import logger

class Encryptor:
    def __init__(self, key=None):
        self.key = key or Fernet.generate_key()
        self.cipher = Fernet(self.key)
        logger.debug("Encryption initialized with secure key.")

    def encrypt(self, data: bytes) -> bytes:
        return self.cipher.encrypt(data)

    def decrypt(self, token: bytes) -> bytes:
        return self.cipher.decrypt(token)
