/// <summary>
/// [mobile]
/// Contact Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 06. 28
/// @version        : 0.2
/// @update
///     v0.1 (2023. 06. 27) : 최초 생성
///     v0.2 (2023. 06. 28) : ImageLoader 적용
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class ContactItemM : InfiniteScrollItem
    {
        [Header("photo")]
        [SerializeField] private RawImage _imgPhoto;

        [Header("text")]
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtDesc;

        [Header("button")]
        [SerializeField] private Button _btnMenu;

        // local variables
        private ImageLoader imageLoader;
        private int seq;
        private string photoPath;


    #region Unity functions

        private void Awake() 
        {
            _btnMenu.onClick.AddListener(OnSelect);

            imageLoader = _imgPhoto.GetComponent<ImageLoader>();
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
            _txtDesc.text = data.info.description;
            _txtDesc.gameObject.SetActive(! string.IsNullOrEmpty(data.info.description));

            if (string.IsNullOrEmpty(photoPath)) 
            {
                imageLoader.LoadProfile(string.Empty, seq).Forget();
            }
            else 
            {
                string url = $"{URL.SERVER_PATH}{photoPath}";
                imageLoader.LoadProfile(url, seq).Forget();
            }
        }

    #endregion  // GPM functions
    }
}