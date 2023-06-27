/// <summary>
/// [mobile]
/// Contact Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 06. 27
/// @version        : 0.1
/// @update
///     v0.1 (2022. 06. 27) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class ContactItemM : InfiniteScrollItem
    {
        [Header("photo")]
        [SerializeField] private RawImage _imgPhoto;
        [SerializeField] private Texture2D _texDefault;
        [SerializeField] private Vector2 _v2PhotoSize;

        [Header("text")]
        [SerializeField] private Text _txtName;

        [Header("button")]
        [SerializeField] private Button _btnMenu;

        // local variables
        private RectTransform rectPhoto;
        private int seq;
        private string photoPath;


    #region Unity functions

        private void Awake() 
        {
            _btnMenu.onClick.AddListener(OnSelect);

            rectPhoto = _imgPhoto.GetComponent<RectTransform>();
        }

        private void OnDestroy() => _imgPhoto.texture = null;

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData itemData)
        {
            base.UpdateData(itemData);
            ContactData data = (ContactData) itemData;

            this.seq = data.info.seq;
            this.photoPath = data.info.photo;
            _txtName.text = data.info.nickNm;

            if (string.IsNullOrEmpty(photoPath)) 
            {
                _imgPhoto.texture = _texDefault;
                Util.ResizeRawImage(rectPhoto, _imgPhoto, _v2PhotoSize.x, _v2PhotoSize.y);
                return;
            }


            // check 'R'
            Texture2D t = R.singleton.GetPhoto(seq);
            if (t != null) 
            {
                _imgPhoto.texture = t;
                Util.ResizeRawImage(rectPhoto, _imgPhoto, _v2PhotoSize.x, _v2PhotoSize.y);
                return;
            }


            string url = $"{URL.SERVER_PATH}{photoPath}";
            GetTexture(url).Forget();
        }

    #endregion  // GPM functions


    #region other function

        private async UniTaskVoid GetTexture(string url) 
        {
            Texture2D res = await NetworkTask.GetTextureAsync(url);
            if (res == null)
            {
                _imgPhoto.texture = _texDefault;
                return;
            }

            _imgPhoto.texture = null;

            res.hideFlags = HideFlags.HideAndDontSave;
            res.filterMode = FilterMode.Point;
            res.Apply();

            R.singleton.AddPhoto(this.seq, res);
            _imgPhoto.texture = res;
            Util.ResizeRawImage(rectPhoto, _imgPhoto, _v2PhotoSize.x, _v2PhotoSize.y);
        }

    #endregion  // other function
    }
}