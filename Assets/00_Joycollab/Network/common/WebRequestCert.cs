using System.Security.Cryptography.X509Certificates;

namespace Joycollab.v2
{
    public class WebRequestCert : UnityEngine.Networking.CertificateHandler
    {
        // reference. https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=croed&logNo=221532255898
        // public string publicKey;

        protected override bool ValidateCertificate(byte[] certificateData)
        {
            /**
            X509Certificate2 cert = new X509Certificate2(certificateData);
            string pk = cert.GetPublicKeyString();

            if (pk.ToLower().Equals(publicKey.ToLower())) 
                return true;
            else 
                return false;
             */

            return true;
        }
    }
}