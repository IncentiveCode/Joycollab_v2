/// <summary>
/// 서버의 이미지를 불러오는 기능을 전담하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 28
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 28) : 최초 생성, mobile scene 에 적용 실험.
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
        private RectTransform rect;


    #region Unity functions

        private void Awake() 
        {
            img = GetComponent<RawImage>();
            rect = GetComponent<RectTransform>();
            img.texture = _texDefault;
        }

        private void OnDestroy() 
        {
            img.texture = null;
        }

    #endregion  // Unity functions
        

    #region Public function

        public async UniTaskVoid LoadProfile(string url, int seq) 
        {
            // check 'null'
            if (img == null) img = GetComponent<RawImage>();
            if (rect == null) rect = GetComponent<RectTransform>();

            // check 'url'
            if (string.IsNullOrEmpty(url))
            {
                img.texture = _texDefault;
                Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
                return;
            }

            // check 'R'
            Texture2D res = R.singleton.GetPhoto(seq);
            if (res != null) 
            {
                img.texture = res;
                Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
                return;
            }

            // request
            res = await NetworkTask.GetTextureAsync(url);
            if (res == null) 
            {
                img.texture = _texDefault;
                Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
                return;
            }

            img.texture = null;

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply();

            R.singleton.AddPhoto(seq, res);
            img.texture = res;
            Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
        }

        public async UniTaskVoid LoadImage(string url) 
        {
            // check 'null'
            if (img == null) img = GetComponent<RawImage>();
            if (rect == null) rect = GetComponent<RectTransform>();

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

            img.texture = null;

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply();

            img.texture = res;
            Util.ResizeRawImage(rect, img, _v2Size.x, _v2Size.y);
        }

    #endregion  // Public function
    }
}