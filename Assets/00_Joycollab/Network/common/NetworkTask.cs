/// <summary>
/// Network 통신 라이브러리  
/// @author         : HJ Lee
/// @last update    : 2023. 02. 20
/// @version        : 1.0
/// @update
///     v1.0 (2023. 02. 20) : UniTask 사용해서 최초 생성.
/// </summary>

using System;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class PsResponse<T>
    {
        public long code { get; }
        public string message { get; }
        public T data { get; }
        public string optionData { get; }

        public PsResponse(long code, T data) 
        {
            this.code = code; 
            this.message = string.Empty;
            this.data = data;
            this.optionData = string.Empty;
        }

        public PsResponse(long code, string message) 
        {
            this.code = code;
            this.message = message;
            this.data = default(T);
            this.optionData = string.Empty;
        }

        public PsResponse(long code, string message, string optionData) 
        {
            this.code = code;
            this.message = message;
            this.data = default(T);
            this.optionData = optionData;
        }
    }

    public class NetworkTask
    {
        // timeout seconds
        protected static double TIMEOUT_DEFAULT = 3;
        protected static double TIMEOUT_WEATHER = 10;
        protected static double TIMEOUT_TEXTURE = 15;

        // HTTP Status Code
        protected const int HTTP_EXCEPTION = -1; 
        public const int HTTP_STATUS_CODE_OK = 200;
        protected const int HTTP_STATUS_CODE_CREATED = 201;
        public const int HTTP_STATUS_CODE_NO_CONTENT = 204;
        protected const int HTTP_STATUS_CODE_BAD_REQUEST = 400;
        protected const int HTTP_STATUS_CODE_UNAUTHORIZED = 401;
        protected const int HTTP_STATUS_CODE_FORBIDDEN = 403;
        protected const int HTTP_STATUS_CODE_NOT_FOUND = 404;
        protected const int HTTP_STATUS_CODE_NOT_ACCEPTABLE = 406;
        protected const int HTTP_STATUS_CODE_CONFLICT = 409;
        protected const int HTTP_STATUS_CODE_GONE = 410;
        protected const int HTTP_STATUS_CODE_SERVER_ERROR = 500;

        // common strings in header
        protected const string ACCEPT_LANGUAGE = "Accept-Language";
        protected const string CONTENT_TYPE = "Content-Type";
        protected const string CONTENT_JSON = "application/json";
        protected const string AUTHORIZATION = "Authorization";

        // common strings in Request
        public const string BASIC_TOKEN = "Basic YWRtOmdhbnNpbmk=";
        public const string MOBILE_BASIC_TOKEN = "Basic YXBwOmdhbnNpbmk=";
        public const string GRANT_TYPE = "grant_type";
        public const string GRANT_TYPE_PW = "password";
        public const string GRANT_TYPE_REFRESH = "refresh_token";
        public const string PASSWORD = "password";
        public const string REFRESH_TOKEN = "refresh_token";
        public const string SCOPE = "scope";
        public const string SCOPE_ADM = "adm";
        public const string SCOPE_APP = "app";
        public const string USERNAME = "username"; 


    #region common
        private static async UniTask CheckConnection() 
        {
            if (Application.internetReachability == NetworkReachability.NotReachable) 
            {
                Debug.LogError("Network is not connected.");

                await UniTask.WaitUntil(() => Application.internetReachability != NetworkReachability.NotReachable);
                Debug.LogError("Network is connected.");
            }
        }

        private static string HandleError(long code, string data) 
        {
            string result = "";
            switch (code) 
            {
                case HTTP_STATUS_CODE_BAD_REQUEST : 
                    ResBadRequest badRequest = JsonUtility.FromJson<ResBadRequest>(data); 
                    string msg = badRequest.Message;
                    if (string.IsNullOrEmpty(msg)) 
                    {
                        SimpleError err = JsonUtility.FromJson<SimpleError>(data);
                        result = err.error_description;
                    }
                    else 
                    {
                        result = msg;
                    }
                    // Debug.LogError("  error : "+ badRequest.Error);
                    // Debug.LogError("  message : "+ badRequest.Message);
                    break;

                case HTTP_STATUS_CODE_UNAUTHORIZED :
                    SimpleError simpleError = JsonUtility.FromJson<SimpleError>(data);
                    // Debug.LogError("  error : "+ simpleError.error); 
                    // Debug.LogError("  desc : "+ simpleError.error_description);
                    result = simpleError.error_description;
                    break;

                case HTTP_STATUS_CODE_FORBIDDEN : 
                    // Debug.LogError("  error : "+ data);
                    result = data;
                    break;

                case HTTP_STATUS_CODE_NOT_FOUND :  
                    // Debug.LogError("  Not Found");
                    result = "Not Found";
                    break;

                case HTTP_STATUS_CODE_NOT_ACCEPTABLE :
                    // Debug.LogError("  Not Acceptable");
                    result = data;
                    break;

                case HTTP_STATUS_CODE_CONFLICT :
                    // Debug.LogError("  Conflict");
                    result = data;
                    break;
                
                case HTTP_STATUS_CODE_GONE :
                    // Debug.LogError("  Gone");
                    result = data;
                    break;

                case HTTP_STATUS_CODE_SERVER_ERROR :  
                    // Debug.LogError("  Internal Server Error - "+ data);
                    result = "Internal Server Error";
                    break;
            }

            return result;
        }
    #endregion  // common 


    #region Post only
        public static async UniTask<PsResponse<T>> PostAsync<T>(string url, WWWForm body, string contentType="", string token="") 
        {
            // 0. test
            // await UniTask.Delay(TimeSpan.FromSeconds(2));

            // 1. network check
            await CheckConnection();

            // 2. timeout setting
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(TIMEOUT_DEFAULT));

            // 3. create UnityWebRequest
            UnityWebRequest req = UnityWebRequest.Post(url, body); 
            req.certificateHandler = new WebRequestCert();
            req.useHttpContinue = false;
            req.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            // 4. set header
            // req.SetRequestHeader(ACCEPT_LANGUAGE, LanguageManager.Instance.Region);
            req.SetRequestHeader(ACCEPT_LANGUAGE, "ko");
            if (!string.IsNullOrEmpty(contentType))
            {
                req.SetRequestHeader(CONTENT_TYPE, contentType);
            }
            if (!string.IsNullOrEmpty(token)) 
            {
                req.SetRequestHeader(AUTHORIZATION, token);
            }

            // 5. request to server
            try 
            {
                var res = await req.SendWebRequest().WithCancellation(cts.Token);
                long code = req.responseCode;
                string data = res.downloadHandler.text;

                if (data.Length == 0)
                {
                    return new PsResponse<T>(code, string.Empty);
                }

                // json 형태가 아니라면, data 를 그대로 리턴
                string first = data.Substring(0, 1);
                if (! first.Equals("[") && ! first.Equals("{")) 
                {
                    return new PsResponse<T>(code, string.Empty, data);
                }

                // 첫글자가 [ 로 시작하면, list 를 붙여준다.
                if (first.Equals("["))
                {
                    data = "{\"list\":" + data + "}";
                }

                T result = JsonUtility.FromJson<T>(data);
                return new PsResponse<T>(code, result);
            }
            catch (OperationCanceledException ce) 
            {
                if (ce.CancellationToken == cts.Token) 
                {
                    Debug.LogError("NetworkTask | PostAsync() timeout exception : "+ ce.Message);

                    // re-try
                    return await PostAsync<T>(url, body, contentType, token);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"NetworkTask | PostAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {req.downloadHandler.text}
                        - Exception : {e.Message}");

                long code = req.responseCode;
                string message = HandleError(req.responseCode, req.downloadHandler.text);
                if (message.Length == 0)
                {
                    return new PsResponse<T>(code, string.Empty);
                }

                return new PsResponse<T>(code, message);
            }

            return new PsResponse<T>(HTTP_EXCEPTION, "알 수 없는 오류");
        }
    #endregion


    #region File, Image and ETC
        public static async UniTask<Texture2D> GetTextureAsync(string url) 
        {
            // 1. network check
            await CheckConnection();

            // 2. timeout setting
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(TIMEOUT_TEXTURE));

            // 3. create UnityWebRequest
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            req.certificateHandler = new WebRequestCert();
            req.useHttpContinue = false;

            // 4. request to server
            try 
            {
                await req.SendWebRequest().WithCancellation(cts.Token);
                long code = req.responseCode;

                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                return texture;
            }
            catch (OperationCanceledException ce) 
            {
                if (ce.CancellationToken == cts.Token) 
                {
                    Debug.LogError("NetworkTask | GetTextureAsync() timeout exception : "+ ce.Message);

                    // re-try
                    return await GetTextureAsync(url);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"NetworkTask | GetTextureAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {e.Message}");

                return null;
            }

            return null;
        }
    #endregion  // File, Image and ETC


    #region Common Request
        public static async UniTask<PsResponse<T>> RequestAsync<T>(string url, MethodType type, string body="", string token="") 
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
            return await RequestAsync<T>(url, type, bodyRaw, token);
        }
    
        private static async UniTask<PsResponse<T>> RequestAsync<T>(string url, MethodType type, byte[] bodyRaw, string token="") 
        {
            // 0. test
            // await UniTask.Delay(TimeSpan.FromSeconds(2f));

            // 1. network check
            await CheckConnection();

            // 2. timeout setting
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(TIMEOUT_DEFAULT));

            // 3. create UnityWebRequest
            UnityWebRequest req = new UnityWebRequest(url, type.ToString()); 
            req.certificateHandler = new WebRequestCert();
            req.useHttpContinue = false;
            req.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            // 4. set header
            // req.SetRequestHeader(ACCEPT_LANGUAGE, LanguageManager.Instance.Region);
            req.SetRequestHeader(ACCEPT_LANGUAGE, "ko");
            req.SetRequestHeader(CONTENT_TYPE, CONTENT_JSON);
            if (!string.IsNullOrEmpty(token)) 
            {
                req.SetRequestHeader(AUTHORIZATION, token);
            }
            
            // 5. set body
            if (bodyRaw.Length != 0)
            {
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            // 6. request to server
            try 
            {
                var res = await req.SendWebRequest().WithCancellation(cts.Token);
                long code = req.responseCode;
                string data = res.downloadHandler.text;

                if (data.Length == 0)
                {
                    return new PsResponse<T>(code, string.Empty);
                }

                // json 형태가 아니라면, data 를 그대로 리턴한다.
                string first = data.Substring(0, 1);
                if (! first.Equals("[") && ! first.Equals("{")) 
                {
                    return new PsResponse<T>(code, string.Empty, data);
                }

                // 첫글자가 [ 로 시작하면, list 를 붙여준다.
                if (first.Equals("["))
                {
                    data = "{\"list\":" + data + "}";
                }

                T result = JsonUtility.FromJson<T>(data);
                return new PsResponse<T>(code, result);
            }
            catch (OperationCanceledException ce) 
            {
                if (ce.CancellationToken == cts.Token) 
                {
                    Debug.LogError("NetworkTask | RequestAsync() timeout exception : "+ ce.Message);

                    // re-try
                    return await RequestAsync<T>(url, type, bodyRaw, token);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"NetworkTask | RequestAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {req.downloadHandler.text}
                        - Exception : {e.Message}");

                long code = req.responseCode;
                string message = HandleError(req.responseCode, req.downloadHandler.text);
                if (message.Length == 0)
                {
                    return new PsResponse<T>(code, string.Empty);
                }

                return new PsResponse<T>(code, message);
            }

            return new PsResponse<T>(HTTP_EXCEPTION, "알 수 없는 오류");
        }

        // TODO. 토큰 갱신 함수 추가 예정
    #endregion  // Common Request
    }
}