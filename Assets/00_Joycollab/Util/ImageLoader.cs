/// <summary>
/// 서버의 이미지를 불러오는 기능을 전담하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 11. 08
/// @version        : 0.2
/// @update
///     v0.1 (2023. 06. 28) : 최초 생성, mobile scene 에 적용 실험.
///     v0.2 (2023. 11. 08) : Resize() 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
	[RequireComponent(typeof(RawImage))]
    public class ImageLoader : MonoBehaviour
    {
        private const string TAG = "ImageLoader";

        [SerializeField] private Vector2 _v2Size;
        [SerializeField] private Texture2D _texDefault;

        // local variables
        private RawImage img;
        private Texture2D res;
        private RectTransform rect;


    #region Unity functions

        private void Awake() 
        {
            img = GetComponent<RawImage>();
            rect = GetComponent<RectTransform>();
            img.texture = _texDefault;
        }

        /**
        private void OnDestroy() 
        {
            if (res != null)
            {
                Destroy(res);
            }
        }
         */

    #endregion  // Unity functions
        

    #region Public function

        public async UniTaskVoid LoadProfile(string url, int seq) 
        {
            // check 'null'
            if (img == null) img = GetComponent<RawImage>();
            if (img == null) 
            {
                Debug.Log($"{TAG} | LoadProfile(), raw image 를 확인할 수 없음.");
                return;
            }
            if (rect == null) rect = GetComponent<RectTransform>();

            // clear
            // img.texture = null;

            // check 'url'
            Debug.Log($"{TAG} | LoadProfile(), step 1 - check url : {url}"); 
            if (string.IsNullOrEmpty(url))
            {
                img.texture = _texDefault;
                Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
                return;
            }

            // check 'R'
            Debug.Log($"{TAG} | LoadProfile(), step 2 - photo in R (check)"); 
            // Texture2D res = R.singleton.GetPhoto(url);
            Texture2D res = R.singleton.GetPhoto(seq);
            if (res != null) 
            {
                Debug.Log($"{TAG} | LoadProfile(), step 2 (1) - photo in R (is exist)"); 
                img.texture = res;
                Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
                return;
            }
            else 
            {
                Debug.Log($"{TAG} | LoadProfile(), step 2 (2) - photo in R (is not exist)"); 
            }

            // request
            Debug.Log($"{TAG} | LoadProfile(), step 3 - request photo"); 
            res = await NetworkTask.GetTextureAsync(url);
            if (res == null) 
            {
                img.texture = _texDefault;
                Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
                return;
            }

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply();

            img.texture = res;
            Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);

            // R.singleton.AddPhoto(url, res);
            R.singleton.AddPhoto(seq, res);
        }

        public async UniTaskVoid LoadImage(string url) 
        {
            // check 'null'
            if (img == null) img = GetComponent<RawImage>();
            if (img == null) 
            {
                Debug.Log($"{TAG} | LoadImage(), raw image 를 확인할 수 없음.");
                return;
            }
            if (rect == null) rect = GetComponent<RectTransform>();

            // clear
            // img.texture = null;

            // check 'url'
            if (string.IsNullOrEmpty(url))
            {
                img.texture = _texDefault;
                Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
                return;
            }

            // request
            Texture2D res = await NetworkTask.GetTextureAsync(url);
            if (res == null) 
            {
                img.texture = _texDefault;
                Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
                return;
            }

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply();

            img.texture = res;
            Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
        }

        public void SetDefault() 
        {
            img.texture = _texDefault;
            Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
        }

        public void Resize() 
        {
            Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
        }

    #endregion  // Public function
    }
}