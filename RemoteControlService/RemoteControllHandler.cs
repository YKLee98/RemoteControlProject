// RemoteControlService/RemoteControlHandler.cs
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
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

        // 원격 제어 서버 비동기 시작
        public async Task StartServerAsync()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _config.RemotePort);
                _listener.Start();
                Logger.Info($"원격 제어 서버가 {_config.RemotePort} 포트에서 시작되었습니다.");

                while (true)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    // 클라이언트 접속 인증 및 세션 핸들링
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"원격 제어 서버 오류: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                // 클라이언트 인증 절차
                if (!Authentication.AuthenticateClient(client))
                {
                    Logger.Warn("클라이언트 인증 실패");
                    client.Close();
                    return;
                }

                Logger.Info("클라이언트 인증 성공, 원격 제어 세션 시작");
                // 화면 캡처 및 입력 처리 루프 시작 (간단히 설명)
                // TODO: 캡처한 화면 데이터 전송, 클라이언트로부터 입력 수신하여 Windows 메시지 전송
                await Task.Delay(100); // 실제 구현에서는 반복 루프가 필요함
            }
            catch (Exception ex)
            {
                Logger.Error($"클라이언트 처리 오류: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        // 외부에서 호출하여 원격 세션 시작을 트리거하는 정적 메서드
        public static void TriggerRemoteSession()
        {
            RemoteSessionTriggered?.Invoke();
        }
    }
}
