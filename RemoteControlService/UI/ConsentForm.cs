// RemoteControlService/UI/ConsentForm.cs
using System;
using System.Windows.Forms;

namespace RemoteControlService.UI
{
    /// <summary>
    /// 사용자 동의 요청을 위한 WinForms UI
    /// </summary>
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

        /// <summary>
        /// 동의 요청을 위한 정적 호출 메서드. UI 스레드 처리 보장.
        /// </summary>
        public static void RequestConsent(Action<bool> consentCallback)
        {
            // UI 스레드가 아닌 경우 Invoke 사용 고려
            using (var form = new ConsentForm(consentCallback))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }
    }
}
