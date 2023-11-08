/// <summary>
/// [world]
/// To-Do Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 07. 25
/// @version        : 0.2
/// @update
///     v0.1 (2022. 06. 14) : 최초 생성
///     v0.2 (2022. 07. 25) : detail 로 넘어갈 때, string 이 아니라 int 형 인자를 넘기도록 수정.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class UserItem : InfiniteScrollItem
    {
        private const string TAG = "UserItem";

        [Header("area")]
        [SerializeField] private Button _btnItem;

        [Header("profile")]
        [SerializeField] private RawImage _imgProfile;

        [Header("text")]
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtDesc;

        // local variables
        private ImageLoader loader;
        private WorldAvatarData data;


    #region Unity functions

        private void Awake() 
        {
            // set button listener
            _btnItem.onClick.AddListener(OnSelect);


            // set local variables
            loader = _imgProfile.GetComponent<ImageLoader>();
        }

    #endregion  // Unity functions
        

    #region GPM functions

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);
            this.data = (WorldAvatarData) scrollData;

            // set profile
            string url = $"{URL.SERVER_PATH}{data.info.photo}";
            loader.LoadProfile(url, data.info.seq).Forget();

            // set text 
            _txtName.text = data.info.nickNm;
            _txtDesc.text = $"회사명과 직급은 준비 중 입니다.";
        }

    #endregion  // GPM functions
    }
}