// RemoteControlService/UI/ConsentForm.cs
using System;
using System.Windows.Forms;

namespace RemoteControlService.UI
{
    public partial class ConsentForm : Form
    {
        private Action<bool> _consentCallback;

        public ConsentForm(Action<bool> consentCallback)
        {
            InitializeComponent();
            _consentCallback = consentCallback;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            _consentCallback?.Invoke(true);
            this.Close();
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            _consentCallback?.Invoke(false);
            this.Close();
        }

        // 정적 호출 메서드. 다른 스레드에서 UI 호출 보장이 필요할 경우 Invoke 사용
        public static void RequestConsent(Action<bool> consentCallback)
        {
            // UI 스레드에서 실행되도록 처리
            var form = new ConsentForm(consentCallback);
            form.ShowDialog();
        }
    }
}
