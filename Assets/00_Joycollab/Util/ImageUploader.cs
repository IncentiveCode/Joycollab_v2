/// <summary>
/// 이미지 업로드 담당 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 07. 04
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 21) : 최초 생성, login scene 에 적용 실험.
///     v0.2 (2023. 06. 28) : mobile scene 적용 실험. 
///     v0.3 (2023. 07. 04) : file name 을 획득할 수 있도록 기능 수정.
/// </summary>

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class ImageUploader : MonoBehaviour, IPointerDownHandler
    {
        private const string TAG = "ImageUploader";

        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private RawImage _imgUpload;

        // local variables
        public bool Interactable { get; set; }
        private int width, height;
        private RectTransform rect; 

        // for upload        
        private string _base64Header;
        private string _encodedString;
        public string encodedString {
            get { return _encodedString; }
        }
        public string imageInfo {
            get { return _base64Header + _encodedString; }
        }

        private string _imageUrl, _imageName;
        public string ImageUrl {
            get { return _imageUrl; } 
        }
        public string ImageName {
            get { return _imageName; }
            set { _imageName = value; }
        }

    #if UNITY_WEBGL && !UNITY_EDITOR

        [DllImport("__Internal")] private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    #endif


    #region Unity functions

        private void OnDestroy() 
        {
            Clear();
        }

    #endregion


    #region Public functions

        public void Init(int width=128, int height=128)
        {
            Interactable = true;

            this.width = width;
            this.height = height;

            rect = _imgUpload.GetComponent<RectTransform>();
        }

        public void Clear() 
        {
            rect.sizeDelta = new Vector2(width, height);

            _base64Header = _encodedString = string.Empty;
            _imageUrl = _imageName = string.Empty;
        }

        public void OnImageUpload(string data) 
        {
            Debug.Log($"{TAG} | OnImageUpload(), data : "+ data);
            string[] arr = data.Split('|');
            string url = arr[0];
            string name = arr[1];

            string[] temp = url.Split('/');
            _imageName = temp[temp.Length - 1];

            if (! string.IsNullOrEmpty(url))
            {
                GetImageTexture(url, name).Forget();
            }
        }

    #endregion


    #region Private functions

        private async UniTaskVoid GetImageTexture(string url, string name)
        {
            if (_imgUpload == null) 
            {
                Debug.LogError($"{TAG} | 타겟 설정이 먼저 되어야 합니다.");
                return;
            }

            string path = string.Empty;
            string ext = string.Empty;

        #if UNITY_WEBGL && !UNITY_EDITOR 
            path = url;
            ext = ".png";
        #else
            path = "file://"+ url;
            ext = System.IO.Path.GetExtension(url);
        #endif

            _imageUrl = path;
            Texture2D res = await NetworkTask.GetTextureAsync(path);
            if (res == null) 
            {
                Debug.LogError($"{TAG} | 이미지 로딩 실패.");
                return;
            }

            if (_inputName != null) 
            {
                _inputName.text = name;
            }

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply();

            Debug.Log($"{TAG} | ResizeTest() - path : {path}, ext : {ext}, width : {width}, height : {height}");
            _imgUpload.texture = res;
            Util.ResizeRawImage(rect, _imgUpload, this.width, this.height);
            
            byte[] bytes; 
            if (ext.Equals(".png"))
            {
                bytes = res.EncodeToPNG();
                _base64Header = "data:image/png;base64,";
                _encodedString = System.Convert.ToBase64String(bytes);
            }
            else if (ext.Equals(".jpg")) 
            {
                bytes = res.EncodeToJPG();
                _base64Header = "data:image/jpg;base64,";
                _encodedString = System.Convert.ToBase64String(bytes);
            }
        }

    #endregion


    #region Interface functions

        public void OnPointerDown(PointerEventData data) 
        {
        #if UNITY_WEBGL && !UNITY_EDITOR

            Debug.Log($"{TAG} | OnPointerDown() call. object name : {gameObject.name}");
            UploadFile(gameObject.name, "OnImageUpload", ".png, .jpg", false);

        #elif UNITY_EDITOR

            string url = EditorUtility.OpenFilePanelWithFilters("사진 선택", "", new string[]{"Image files", "png,jpg"});
            if (! string.IsNullOrEmpty(url)) 
            {
                string[] temp = url.Split('/');
                _imageName = temp[temp.Length - 1];
                GetImageTexture(url, _imageName).Forget();
            }
            else 
            {
                Debug.Log($"{TAG} | 취소 되었습니다.");
            }

        #endif
        }

    #endregion
    }
}