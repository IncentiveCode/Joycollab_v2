using System.Security.Cryptography.X509Certificates;

public class WebRequestCert : UnityEngine.Networking.CertificateHandler
{
    // public string publicKey;

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        /*
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
