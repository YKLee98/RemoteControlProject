// RemoteControlService/Authentication.cs
using System;
using System.IO;
using System.Text;
using System.Net.Security;
using Common.Utils;

namespace RemoteControlService
{
    /// <summary>
    /// 클라이언트 인증 모듈 (예제: 사전 공유 키 방식)
    /// </summary>
    public static class Authentication
    {
        /// <summary>
        /// 네트워크 스트림을 통해 클라이언트의 인증 요청을 처리하여 사전 공유 키 확인
        /// </summary>
        /// <param name="stream">인증 통신에 사용되는 스트림 (TLS 적용 가능)</param>
        /// <param name="expectedKey">사전 공유 키</param>
        /// <returns>인증 성공 여부</returns>
        public static bool AuthenticateClient(System.IO.Stream stream, string expectedKey)
        {
            try
            {
                // 클라이언트가 인증 키를 보내면 이를 확인하는 단순 프로토콜 (실제 구현 시, Challenge-Response 사용 권장)
                byte[] authBuffer = new byte[256];
                int bytesRead = stream.Read(authBuffer, 0, authBuffer.Length);
                string receivedKey = Encoding.UTF8.GetString(authBuffer, 0, bytesRead).Trim();

                if (receivedKey == expectedKey)
                {
                    byte[] ack = Encoding.UTF8.GetBytes("AUTH_OK");
                    stream.Write(ack, 0, ack.Length);
                    Logger.Info("클라이언트 인증 성공");
                    return true;
                }
                else
                {
                    byte[] nack = Encoding.UTF8.GetBytes("AUTH_FAIL");
                    stream.Write(nack, 0, nack.Length);
                    Logger.Warn("클라이언트 인증 실패");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("클라이언트 인증 오류: " + ex.Message);
                return false;
            }
        }
    }
}
