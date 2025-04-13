// ClientApp/UI/RemoteControlViewer.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using Common.Utils;

namespace ClientApp.UI
{
    public partial class RemoteControlViewer : Form
    {
        private TcpClient _client;
        public RemoteControlViewer()
        {
            InitializeComponent();
        }

        // 원격 서버 연결 및 화면 스트리밍 시작
        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync("서버IP주소", 9000);  // Config.json의 RemotePort와 일치시킴
                Logger.Info("원격 서버에 연결되었습니다.");

                // TODO: 서버로부터 실시간 화면 데이터를 받아 PictureBox에 표시하고,
                // 마우스/키보드 이벤트를 캡처하여 서버에 전송하는 로직 구현
            }
            catch (Exception ex)
            {
                Logger.Error($"연결 오류: {ex.Message}");
                MessageBox.Show("서버 연결에 실패하였습니다.");
            }
        }
    }
}
