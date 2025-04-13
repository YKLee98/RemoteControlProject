// RemoteControlService/RemoteControlHandler.cs
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Net.Security;
using Common.Models;
using Common.Utils;

namespace RemoteControlService
{
    public class RemoteControlHandler
    {
        private readonly ServiceConfig _config;
        private TcpListener _listener;
        public static event Action RemoteSessionTriggered;

        public RemoteControlHandler(ServiceConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 비동기 원격 제어 서버 시작 (TLS 사용 여부에 따라 분기)
        /// </summary>
        public async Task StartServerAsync()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _config.RemotePort);
                _listener.Start();
                Logger.Info($"원격 제어 서버가 포트 {_config.RemotePort}에서 시작되었습니다.");

                while (true)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Logger.Info("새 클라이언트 접속 요청 수신");

                    // 비동기 클라이언트 처리
                    _ = Task.Run(() => HandleClientAsync(client));
                }
            }
            catch (Exception ex)
            {
                Logger.Error("원격 제어 서버 오류: " + ex.Message);
            }
        }

        /// <summary>
        /// 클라이언트 연결 처리 – TLS 적용 및 인증 수행
        /// </summary>
        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                try
                {
                    // TLS 적용 여부에 따라 스트림 결정
                    var networkStream = client.GetStream();

                    if (_config.TlsEnabled)
                    {
                        // 서버 인증서 로딩
                        X509Certificate2 serverCertificate = new X509Certificate2(_config.CertificatePath, _config.CertificatePassword);
                        SslStream sslStream = new SslStream(networkStream, false);
                        await sslStream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired: false,
                            enabledSslProtocols: SslProtocols.Tls12, checkCertificateRevocation: true);
                        networkStream = sslStream;
                        Logger.Info("TLS 암호화 채널 수립 완료");
                    }

                    // 클라이언트 인증 (사전 공유 키 방식 예제)
                    bool authResult = Authentication.AuthenticateClient(networkStream, _config.AuthenticationKey);
                    if (!authResult)
                    {
                        Logger.Warn("클라이언트 인증 실패. 연결 종료합니다.");
                        return;
                    }

                    Logger.Info("클라이언트 인증 성공. 원격 제어 세션 시작.");

                    // 원격 제어 세션 처리 (화면 캡처, 입력 이벤트 처리 루프)
                    // TODO: 화면 캡처 및 스트리밍, 클라이언트 입력 이벤트 수신 및 처리 로직 구현
                    await ProcessRemoteSessionAsync(networkStream);
                }
                catch (Exception ex)
                {
                    Logger.Error("클라이언트 처리 중 오류: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 원격 제어 세션을 처리하는 루프 (추후 화면 스트리밍 및 입력 이벤트 연동)
        /// </summary>
        private async Task ProcessRemoteSessionAsync(System.IO.Stream stream)
        {
            // 예제: 단순 핑-퐁 메시지 교환 (실제 구현 시, 화면 데이터와 입력 이벤트 처리)
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string received = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Logger.Info("클라이언트로부터 메시지 수신: " + received);

                // 응답 전송
                string response = "원격 제어 세션 시작 승인됨";
                byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            catch (Exception ex)
            {
                Logger.Error("원격 세션 처리 오류: " + ex.Message);
            }
        }

        /// <summary>
        /// 외부에서 원격 세션을 시작하도록 호출 가능한 정적 메서드
        /// </summary>
        public static void TriggerRemoteSession()
        {
            RemoteSessionTriggered?.Invoke();
        }
    }
}

