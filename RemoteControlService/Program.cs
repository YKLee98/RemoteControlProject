// RemoteControlService/Program.cs
using System;
using System.Threading.Tasks;
using System.IO;
using Common.Utils;

namespace RemoteControlService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logger.Init("RemoteControlService.log");
            Console.WriteLine("원격 제어 서비스 시작...");

            // 서비스 설정 로드
            var config = ConfigLoader.Load("Config.json");
            Logger.Info("설정 파일 로드 완료.");

            // 파일 접근 모니터링 시작
            var monitor = new Monitor(config.ExcelFilePath);
            monitor.FileAccessed += OnExcelFileAccessed;
            monitor.Start();

            // 원격 제어 서버 시작
            var remoteHandler = new RemoteControlHandler(config);
            await remoteHandler.StartServerAsync();

            // 프로그램 종료 전까지 대기
            Console.WriteLine("서비스가 실행 중입니다. 종료하려면 아무 키나 누르세요...");
            Console.ReadKey();
        }

        // Excel 파일 접근 시 이벤트 핸들러
        private static void OnExcelFileAccessed(object sender, string filePath)
        {
            Logger.Info($"Excel 파일 접근 이벤트 감지: {filePath}");
            // 동의 요청 UI 호출 (비동기 호출하여 UI 스레드에서 실행)
            UI.ConsentForm.RequestConsent((consented) =>
            {
                if (consented)
                {
                    Logger.Info("사용자 동의 확인됨. 원격 제어 세션을 시작합니다.");
                    // 동의 후 원격 제어 세션 활성화 (이미 실행 중인 서버에 연결 안내 등)
                    RemoteControlHandler.TriggerRemoteSession();
                }
                else
                {
                    Logger.Warn("사용자가 원격 제어 요청을 거부하였습니다.");
                }
            });
        }
    }
}
