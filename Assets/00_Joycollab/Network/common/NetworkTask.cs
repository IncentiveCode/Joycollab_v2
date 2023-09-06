/// <summary>
/// Network 통신 라이브러리  
/// @author         : HJ Lee
/// @last update    : 2023. 09. 06
/// @version        : 0.7
/// @update
///     v0.1 (2023. 02. 20) : UniTask 사용해서 최초 생성.
///     v0.2 (2023. 03. 31) : int 형태로 리턴되는 결과를 처리하기 위해, PsResponse 안에 int 형 data 추가.
///     v0.3 (2023. 04. 03) : Token refresh logic 추가. API 호출시 항상 체크 > 만료시에만 체크.
///     v0.4 (2023. 05. 10) : extra field 추가.
///     v0.5 (2023. 06. 19) : multipart form post method 추가.
///     v0.6 (2023. 07. 31) : return 앞에 req.Dispose() 추가.
///     v0.7 (2023. 09. 06) : 중복되는 req.Dispose() 를 finally 로 이동. FileDownload function 추가.
///                           token refresh 가 없는 GoogleDriveAsync() 추가.
/// </summary>

using System;
using System.Collections.Generic;
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
        public string stringData { get; }
        public int intData { get; }
        public string extra { get; set; }

        public PsResponse(long code, T data) 
        {
            this.code = code; 
            this.message = string.Empty;
            this.data = data;
            this.stringData = string.Empty;
            this.intData = -1;
            this.extra = string.Empty;
        }

        public PsResponse(long code, string message) 
        {
            this.code = code;
            this.message = message;
            this.data = default(T);
            this.stringData = string.Empty;
            this.intData = -1;
            this.extra = string.Empty;
        }

        public PsResponse(long code, string message, string strData) 
        {
            this.code = code;
            this.message = message;
            this.data = default(T);
            this.stringData = strData;
            this.intData = -1;
            this.extra = string.Empty;
        }

        public PsResponse(long code, string message, int intData) 
        {
            this.code = code;
            this.message = message;
            this.data = default(T);
            this.stringData = string.Empty;
            this.intData = intData;
            this.extra = string.Empty;
        }
    }

    public class NetworkTask
    {
        private const string TAG = "NetworkTask";

        // timeout seconds
        protected static double TIMEOUT_DEFAULT = 20;
        protected static double TIMEOUT_WEATHER = 20;
        protected static double TIMEOUT_TEXTURE = 30;
        protected static double TIMEOUT_MULTIPART = 180;

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
        public const string CONTENT_JSON = "application/json";
        public const string CONTENT_MULTIPART = "multipart/form-data; boundary=";
        public const string AUTHORIZATION = "Authorization";

        // common strings in Request
        public const string BASIC_TOKEN = "Basic YWRtOmdhbnNpbmk=";
        public const string BASIC_TOKEN_M = "Basic YXBwOmdhbnNpbmk=";
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
            string msg = string.Empty;
            string result = string.Empty;
            switch (code) 
            {
                case HTTP_STATUS_CODE_BAD_REQUEST : 
                    ResBadRequest badRequest = JsonUtility.FromJson<ResBadRequest>(data); 
                    msg = badRequest.Message;
                    if (string.IsNullOrEmpty(msg)) 
                    {
                        SimpleError err = JsonUtility.FromJson<SimpleError>(data);
                        result = err.error_description;
                    }
                    else 
                    {
                        result = msg;
                    }
                    break;

                case HTTP_STATUS_CODE_UNAUTHORIZED :
                    SimpleError simpleError = JsonUtility.FromJson<SimpleError>(data);
                    result = simpleError.error_description;
                    break;

                case HTTP_STATUS_CODE_FORBIDDEN : 
                    result = data;
                    break;

                case HTTP_STATUS_CODE_NOT_FOUND :  
                    ResBadRequest notFound = JsonUtility.FromJson<ResBadRequest>(data); 
                    msg = notFound.Message;
                    if (string.IsNullOrEmpty(msg)) 
                    {
                        result = "Not Found";
                    }
                    else 
                    {
                        result = msg;
                    }
                    break;

                case HTTP_STATUS_CODE_NOT_ACCEPTABLE :
                    result = data;
                    break;

                case HTTP_STATUS_CODE_CONFLICT :
                    result = data;
                    break;
                
                case HTTP_STATUS_CODE_GONE :
                    result = data;
                    break;

                case HTTP_STATUS_CODE_SERVER_ERROR :  
                    result = "Internal Server Error";
                    break;
            }

            return result;
        }

        private static async UniTask<string> RefreshToken() 
        {
            // 1. network check
            await CheckConnection();

            // 2. timeout setting
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(TIMEOUT_DEFAULT));

            WWWForm form = new WWWForm();
            form.AddField(GRANT_TYPE, GRANT_TYPE_REFRESH);
            form.AddField(PASSWORD, string.Empty);
            form.AddField(REFRESH_TOKEN, R.singleton.refreshToken);
            form.AddField(SCOPE, R.singleton.tokenScope);
            form.AddField(USERNAME, R.singleton.ID);

            PsResponse<ResToken> res = await PostAsync<ResToken>(URL.REQUEST_TOKEN, form, string.Empty, 
                R.singleton.tokenScope.Equals(SCOPE_ADM) ? BASIC_TOKEN : BASIC_TOKEN_M);

            if (string.IsNullOrEmpty(res.message)) 
            {
                R.singleton.TokenInfo = res.data;
                JsLib.SetCookie(Key.TOKEN_TYPE, res.data.token_type);
                JsLib.SetCookie(Key.ACCESS_TOKEN, res.data.access_token);

                return string.Empty;
            }
            else 
            {
                Debug.LogError("토큰 재발행 실패.");

                return res.message;
            }
        }

    #endregion  // common 


    #region Post only

        public static async UniTask<PsResponse<T>> PostAsync<T>(string url, WWWForm body, string contentType="", string token="") 
        {
            // 0. test
            Debug.Log("Request url : "+ url);

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
            req.SetRequestHeader(ACCEPT_LANGUAGE, R.singleton.Region);
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
                    Debug.LogError($"{TAG} | PostAsync() timeout exception : {ce.Message}");

                    // re-try
                    return await PostAsync<T>(url, body, contentType, token);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"{TAG} | PostAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {req.downloadHandler.text}
                        - Exception : {e.Message}");

                if (req.responseCode == HTTP_STATUS_CODE_UNAUTHORIZED && ! string.IsNullOrEmpty(R.singleton.refreshToken)) 
                {
                    string t = await RefreshToken();
                    if (string.IsNullOrEmpty(t))
                    {
                        return await PostAsync<T>(url, body, contentType, R.singleton.token);
                    }
                }
                else 
                {
                    long code = req.responseCode;
                    string message = HandleError(req.responseCode, req.downloadHandler.text);
                    if (message.Length == 0)
                    {
                        return new PsResponse<T>(code, e.Message);
                    }

                    return new PsResponse<T>(code, message);
                }
            }
            finally 
            {
                req.Dispose();
            }

            return new PsResponse<T>(HTTP_EXCEPTION, "알 수 없는 오류");
        }

        public static async UniTask<PsResponse<T>> PostMultipartAsync<T>(string url, List<IMultipartFormSection> body, string token) 
        {
            // 0. test
            Debug.Log("Request url : "+ url);

            // 1. network check
            await CheckConnection();

            // 2. timeout setting
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(TIMEOUT_MULTIPART));

            // 3. set multipart form section process
            List<IMultipartFormSection> origin = new List<IMultipartFormSection>(body);
            byte[] boundary = UnityWebRequest.GenerateBoundary();
            byte[] formSections = UnityWebRequest.SerializeFormSections(origin, boundary);
            byte[] terminate = Encoding.UTF8.GetBytes(string.Concat("\r\n--", Encoding.UTF8.GetString(boundary), "--"));
            byte[] bytes = new byte[formSections.Length + terminate.Length];
            Buffer.BlockCopy(formSections, 0, bytes, 0, formSections.Length);
            Buffer.BlockCopy(terminate, 0, bytes, formSections.Length, terminate.Length);
            string contentType = string.Concat(CONTENT_MULTIPART, Encoding.UTF8.GetString(boundary));

            // 4. create UnityWebRequest
            UnityWebRequest req = UnityWebRequest.Post(url, origin); 
            req.certificateHandler = new WebRequestCert();
            req.useHttpContinue = false;
            req.uploadHandler = (UploadHandler) new UploadHandlerRaw(bytes);
            req.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            // 5. set header
            req.SetRequestHeader(ACCEPT_LANGUAGE, R.singleton.Region);
            req.SetRequestHeader(CONTENT_TYPE, contentType);
            req.SetRequestHeader(AUTHORIZATION, token);

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
                    Debug.LogError($"{TAG} | PostMultipartAsync() timeout exception : {ce.Message}");

                    // re-try
                    return await PostMultipartAsync<T>(url, body, token);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"{TAG} | PostMultipartAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {req.downloadHandler.text}
                        - Exception : {e.Message}");

                if (req.responseCode == HTTP_STATUS_CODE_UNAUTHORIZED && ! string.IsNullOrEmpty(R.singleton.refreshToken)) 
                {
                    string t = await RefreshToken();
                    if (string.IsNullOrEmpty(t))
                    {
                        return await PostMultipartAsync<T>(url, body, R.singleton.token);
                    }
                }
                else 
                {
                    long code = req.responseCode;
                    string message = HandleError(req.responseCode, req.downloadHandler.text);
                    if (message.Length == 0)
                    {
                        return new PsResponse<T>(code, e.Message);
                    }

                    return new PsResponse<T>(code, message);
                }
            }
            finally 
            {
                req.Dispose();
            }

            return new PsResponse<T>(HTTP_EXCEPTION, "알 수 없는 오류");
        }

    #endregion


    #region File, Image and ETC

        public static async UniTask<byte[]> GetFileAsync(string url) 
        {
            // 0. test
            Debug.Log("Request url : "+ url);

            // 1. network check
            await CheckConnection();

            // 2. timeout setting
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(TIMEOUT_MULTIPART));

            // 3. create UnityWebRequest
            UnityWebRequest req = UnityWebRequest.Get(url);
            req.certificateHandler = new WebRequestCert();
            req.useHttpContinue = false;
            req.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            // 4. request to server
            try 
            {
                await req.SendWebRequest().WithCancellation(cts.Token);
                long code = req.responseCode;

                return req.downloadHandler.data;
            }
            catch (OperationCanceledException ce) 
            {
                if (ce.CancellationToken == cts.Token) 
                {
                    Debug.LogError($"{TAG} | GetFileAsync() timeout exception : {ce.Message}");

                    // re-try
                    return await GetFileAsync(url);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"{TAG} | GetFileAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {e.Message}");

                if (req.responseCode == HTTP_STATUS_CODE_UNAUTHORIZED && ! string.IsNullOrEmpty(R.singleton.refreshToken)) 
                {
                    string t = await RefreshToken();
                    if (string.IsNullOrEmpty(t)) 
                    {
                        return await GetFileAsync(url);
                    }
                }
                else 
                {
                    return null;
                }
            }
            finally
            {
                req.Dispose();
            }

            return null;
        }

        public static async UniTask<Texture2D> GetTextureAsync(string url) 
        {
            // 0. test
            Debug.Log("Request url : "+ url);

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
                    Debug.LogError($"{TAG} | GetTextureAsync() timeout exception : {ce.Message}");

                    // re-try
                    return await GetTextureAsync(url);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"{TAG} | GetTextureAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {e.Message}");

                if (req.responseCode == HTTP_STATUS_CODE_UNAUTHORIZED && ! string.IsNullOrEmpty(R.singleton.refreshToken)) 
                {
                    string t = await RefreshToken();
                    if (string.IsNullOrEmpty(t)) 
                    {
                        return await GetTextureAsync(url);
                    }
                }
                else 
                {
                    return null;
                }
            }
            finally
            {
                req.Dispose();
            }

            return null;
        }

    #endregion  // File, Image and ETC


    #region Common Request

        public static async UniTask<PsResponse<T>> RequestAsync<T>(string url, eMethodType type, string body="", string token="") 
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
            return await RequestAsync<T>(url, type, bodyRaw, token);
        }
    
        private static async UniTask<PsResponse<T>> RequestAsync<T>(string url, eMethodType type, byte[] bodyRaw, string token="") 
        {
            // 0. test
            // await UniTask.Delay(TimeSpan.FromSeconds(2f));
            Debug.Log("Request url : "+ url);

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
            req.SetRequestHeader(ACCEPT_LANGUAGE, R.singleton.Region);
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
                    Debug.LogError($"{TAG} | RequestAsync() timeout exception : {ce.Message}");

                    // re-try
                    return await RequestAsync<T>(url, type, bodyRaw, token);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"{TAG} | RequestAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {req.downloadHandler.text}
                        - Exception : {e.Message}");

                if (req.responseCode == HTTP_STATUS_CODE_UNAUTHORIZED && ! string.IsNullOrEmpty(R.singleton.refreshToken)) 
                {
                    string t = await RefreshToken();
                    if (string.IsNullOrEmpty(t)) 
                    {
                        return await RequestAsync<T>(url, type, bodyRaw, R.singleton.token);
                    }
                }
                else 
                {
                    long code = req.responseCode;
                    string message = HandleError(req.responseCode, req.downloadHandler.text);
                    if (message.Length == 0)
                    {
                        return new PsResponse<T>(code, e.Message);
                    }

                    return new PsResponse<T>(code, message);
                }
            }
            finally 
            {
                req.Dispose();
            }

            return new PsResponse<T>(HTTP_EXCEPTION, "알 수 없는 오류");
        }

    #endregion  // Common Request


    #region Google drive request

        private static async UniTask<PsResponse<string>> RefreshGoogleToken() 
        {
            // 1. network check
            await CheckConnection();

            // 2. timeout setting
            var cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(TIMEOUT_DEFAULT));

            // 3. create UnityWebRequest
            string id = "leehj1321@gmail.com";  // TODO. Repo 의 GoogleId 로 변경할 것.
            string url = string.Format(URL.GOOGLE_ACCESS_TOKEN, id);
            PsResponse<string> res = await NetworkTask.GoogleDriveAsync<string>(url, eMethodType.GET);
            return res;
        }

        public static async UniTask<PsResponse<T>> GoogleDriveAsync<T>(string url, eMethodType type, string body="", string token="") 
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
            return await GoogleDriveAsync<T>(url, type, bodyRaw, token);
        }
    
        private static async UniTask<PsResponse<T>> GoogleDriveAsync<T>(string url, eMethodType type, byte[] bodyRaw, string token="") 
        {
            // 0. test
            // await UniTask.Delay(TimeSpan.FromSeconds(2f));
            Debug.Log("Request url : "+ url);

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
            req.SetRequestHeader(ACCEPT_LANGUAGE, R.singleton.Region);
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
                    Debug.LogError($"{TAG} | GoogleDriveAsync() timeout exception : {ce.Message}");

                    // re-try
                    return await GoogleDriveAsync<T>(url, type, bodyRaw, token);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($@"{TAG} | GoogleDriveAsync() occur exception.
                        - URL : {url}
                        - Response Code : {req.responseCode}
                        - Result : {req.downloadHandler.text}
                        - Exception : {e.Message}");

                if (req.responseCode == HTTP_STATUS_CODE_UNAUTHORIZED) 
                {
                    PsResponse<string> googleTokenRes = await RefreshGoogleToken();
                    if (string.IsNullOrEmpty(googleTokenRes.message)) 
                    {
                        string googleToken = $"Bearer {googleTokenRes.stringData}";
                        Debug.Log($"{TAG} | GoogleTokenRefresh : {googleToken}");
                        return await RequestAsync<T>(url, type, bodyRaw, googleToken);
                    }
                }
                else 
                {
                    long code = req.responseCode;
                    string message = HandleError(req.responseCode, req.downloadHandler.text);
                    if (message.Length == 0)
                    {
                        return new PsResponse<T>(code, e.Message);
                    }

                    return new PsResponse<T>(code, message);
                }
            }
            finally 
            {
                req.Dispose();
            }

            return new PsResponse<T>(HTTP_EXCEPTION, "알 수 없는 오류");
        }

    #endregion  // Google drive request
    }
}