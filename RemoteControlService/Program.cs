// RemoteControlService/Program.cs
using System;
using System.IO;
using System.Threading.Tasks;
using RemoteControlService.UI;
using Common.Utils;            // Logger, ConfigLoader
using Common.Models;           // ServiceConfig 모델

namespace RemoteControlService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 초기 로깅 설정 (로그 파일 경로는 필요에 따라 변경)
            Logger.Init("RemoteControlService.log");
            Logger.Info("원격 제어 서비스 시작...");

            try
            {
                // 서비스 설정 로드 (보안 키, 포트 등 포함)
                var config = ConfigLoader.Load<ServiceConfig>("Config.json");
                Logger.Info("설정 파일 로드 완료.");

                // 파일 접근 모니터링 모듈 초기화 및 시작
                using (var monitor = new Monitor(config.ExcelFilePath))
                {
                    monitor.FileAccessed += OnExcelFileAccessed;
                    monitor.Start();

                    // 원격 제어 TCP 서버 시작 (비동기)
                    RemoteControlHandler remoteHandler = new RemoteControlHandler(config);
                    Task serverTask = remoteHandler.StartServerAsync();

                    Logger.Info("모든 서비스가 정상적으로 시작되었습니다.");
                    Console.WriteLine("서비스 실행 중... 종료하려면 ESC 키를 누르세요.");
                    
                    // 단축키 ESC를 누르면 서비스 종료
                    while (true)
                    {
                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                        {
                            Logger.Info("ESC 입력으로 서비스 종료 요청됨.");
                            break;
                        }
                        await Task.Delay(300);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("서비스 초기화 중 오류: " + ex.Message);
            }
            Logger.Info("원격 제어 서비스 종료.");
        }

        // Excel 파일 접근 이벤트 핸들러
        private static void OnExcelFileAccessed(object sender, string filePath)
        {
            Logger.Info($"Excel 파일 접근 이벤트 감지: {filePath}");
            // UI 동기화 필요 시 별도 스레드에서 안전하게 실행
            ConsentForm.RequestConsent((consented) =>
            {
                if (consented)
                {
                    Logger.Info("사용자 동의 확인됨. 원격 제어 세션을 시작합니다.");
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

