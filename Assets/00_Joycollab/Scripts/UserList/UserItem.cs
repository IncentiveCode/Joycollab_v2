/// <summary>
/// [world]
/// To-Do Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 11. 28
/// @version        : 0.3
/// @update
///     v0.1 (2023. 06. 16) : 최초 생성
///     v0.2 (2023. 10. 06) : UI 작업 진행.
///     v0.3 (2023. 11. 28) : WorldAvatarInfo 와 연동해서 회사 정보와 직급 정보 추가.
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
            _txtDesc.text = $"{data.info.compName} / {data.info.jobGrade}";
        }

    #endregion  // GPM functions
    }
}