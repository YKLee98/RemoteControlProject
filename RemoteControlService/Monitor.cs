// RemoteControlService/Monitor.cs
using System;
using System.IO;
using System.Threading;

namespace RemoteControlService
{
    /// <summary>
    /// Excel 파일 접근 및 변경 이벤트 감지를 위한 모듈
    /// IDisposable을 구현하여 자원 해제 보장
    /// </summary>
    public class Monitor : IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private readonly string _filePath;
        private DateTime _lastEventTime;
        private readonly TimeSpan _minEventInterval = TimeSpan.FromSeconds(2);
        private bool _disposed = false;

        public event EventHandler<string> FileAccessed;

        public Monitor(string excelFilePath)
        {
            if (string.IsNullOrEmpty(excelFilePath) || !File.Exists(excelFilePath))
                throw new ArgumentException("감시할 Excel 파일 경로가 유효하지 않습니다.", nameof(excelFilePath));

            _filePath = excelFilePath;
            string directory = Path.GetDirectoryName(excelFilePath);
            string fileName = Path.GetFileName(excelFilePath);

            _watcher = new FileSystemWatcher(directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Error += OnError;
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
            Logger.Info("파일 모니터링 시작: " + _filePath);
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            Logger.Info("파일 모니터링 중지");
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // 중복 이벤트 필터링: 연속된 이벤트에 대해 최소 시간 간격 유지
            if (DateTime.Now - _lastEventTime < _minEventInterval)
                return;

            _lastEventTime = DateTime.Now;
            Logger.Info("파일 변경 이벤트 발생: " + e.FullPath);
            FileAccessed?.Invoke(this, e.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Logger.Error("파일 감시 오류: " + e.GetException().Message);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _watcher.Dispose();
                _disposed = true;
            }
        }
    }
}
