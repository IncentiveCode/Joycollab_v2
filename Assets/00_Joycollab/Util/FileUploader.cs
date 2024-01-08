/// <summary>
/// 파일 업로드 담당 클래스 
/// @author         : HJ Lee
/// @last update    : 2024. 01. 08
/// @version        : 0.1
/// @update
///     v0.1 (2024. 01. 08) : 최초 생성.
/// </summary>

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2 
{
	public class FileUploader : MonoBehaviour, IPointerDownHandler 
	{
		private const string TAG = "FileUploader";

		[SerializeField] private TMP_InputField _inputFileName;
        [SerializeField] private Vector2 _v2Size;
		[SerializeField] private RawImage _imgUpload;
        [SerializeField] private Texture2D _texDefault;

		// local variables
	    private Dictionary<string, string> fileDictionary;
    	private List<int> fileSize;	
		

    #if UNITY_WEBGL && !UNITY_EDITOR

        [DllImport("__Internal")] private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    #endif


	#region Public functions

		public void Init() 
		{

		}

		public void OnFileUpload(string data) 
		{
            if (string.IsNullOrEmpty(data)) 
            {
                Debug.Log($"{TAG} | OnFileUpload(), data is null");
                return;
            }

            Debug.Log($"{TAG} | OnFileUpload(), data : {data}");
            string[] arr = data.Split('|');
            string url = arr[0];
            string name = arr[1];
		}

	#endregion	// Public functions


	#region Private functions



	#endregion	// Private functions


    #region Interface functions

		public void OnPointerDown(PointerEventData data) 
		{
		#if UNITY_WEBGL && !UNITY_EDITOR

            Debug.Log($"{TAG} | OnPointerDown() call. object name : {gameObject.name}");
            UploadFile(gameObject.name, "OnFileUpload", "image/*", false);

        #elif UNITY_ANDROID 

            Debug.Log($"{TAG} | OnPointerDown() call. object name : {gameObject.name}");

            string[] fileTypes = new string[] { "*/*" };
            NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) => {
                if (! path.Equals(""))
                {
                    Debug.Log($"{TAG} android file path : "+ path);
                    string[] temp = path.Split('/');
                    _imageName = temp[temp.Length - 1];
                    GetImageTexture(path, _imageName).Forget();
                }
            },
            fileTypes);

        #elif UNITY_EDITOR

            string url = EditorUtility.OpenFilePanelWithFilters("선택", "", new string[]{"Image files", "png,jpg"});
            if (! string.IsNullOrEmpty(url)) 
            {
                string[] temp = url.Split('/');
                // _imageName = temp[temp.Length - 1];
                // GetImageTexture(url, _imageName).Forget();
            }
            else 
            {
                Debug.Log($"{TAG} | 취소 되었습니다.");
            }

        #endif			

		}

    #endregion	// Interface functions
	}
}