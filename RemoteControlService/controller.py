import threading
from .network_manager import NetworkManager
from .remote_session import RemoteSession
from .logger import logger
from .exceptions import RemoteControlError

class RemoteController:
    def __init__(self, config):
        self.config = config
        self.network_manager = NetworkManager(config)
        self.remote_session = RemoteSession(config)

    def start(self):
        logger.info("Starting remote controller service.")
        try:
            self.network_manager.setup()
            self.remote_session.initialize()
            self._start_threads()
        except Exception as e:
            logger.exception("Failed to start remote controller service.")
            raise RemoteControlError(str(e))

    def _start_threads(self):
        control_thread = threading.Thread(target=self.remote_session.listen_for_commands)
        control_thread.daemon = True
        control_thread.start()
        logger.info("Remote control session listening thread started.")
