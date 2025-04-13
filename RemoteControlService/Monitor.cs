// RemoteControlService/Monitor.cs
using System;
using System.IO;

namespace RemoteControlService
{
    public class Monitor
    {
        private readonly FileSystemWatcher _watcher;
        private readonly string _filePath;

        // 파일 접근 이벤트 시 전달될 이벤트 핸들러 정의
        public event EventHandler<string> FileAccessed;

        public Monitor(string excelFilePath)
        {
            _filePath = excelFilePath;
            string directory = Path.GetDirectoryName(excelFilePath);
            string fileName = Path.GetFileName(excelFilePath);

            _watcher = new FileSystemWatcher(directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // 중복 이벤트를 필터링하는 로직 추가 (예: 시간 임계값)
            FileAccessed?.Invoke(this, e.FullPath);
        }
    }
}
