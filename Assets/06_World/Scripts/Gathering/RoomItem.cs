/// <summary>
/// [world]
/// 모임방 리스트 항목 Script
/// @author         : HJ Lee
/// @last update    : 2023. 09. 13
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 13) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class RoomItem : InfiniteScrollItem
    {
        private const string TAG = "RoomItem";

        [Header("image")]
        [SerializeField] private RawImage _imgLogo;
        
		[Header("button")]
		[SerializeField] private Button _btnEnter;
		[SerializeField] private Button _btnJoin;

		[Header("text")]
		[SerializeField] private TMP_Text _txtTitle;
		[SerializeField] private TMP_Text _txtDesc;
		[SerializeField] private TMP_Text _txtPublicOpt;
		[SerializeField] private TMP_Text _txtOwner;

        // local variables
        private ClasData data;
        private int seq;
        private ImageLoader loader;


    #region Unity functions

        private void Awake() 
        {
            // set button listener
            _btnEnter.onClick.AddListener(OnEnter);
            _btnJoin.onClick.AddListener(OnJoin);


            // init image loader
            loader = _imgLogo.GetComponent<ImageLoader>();
        }

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData scrollData) 
        {
            base.UpdateData(scrollData);

            data = (ClasData) scrollData;
            this.seq = data.info.seq;

            // 이미지 로드
            string logoPath = string.Format("{0}{1}", URL.SERVER_PATH, data.info.logo);
            Debug.Log($"{TAG} | image path : {logoPath}");
            loader.LoadImage(logoPath).Forget();

            // 문구 정리
            _txtTitle.text = data.info.nm;
            _txtDesc.text = data.info.clas.bigo;
            _txtPublicOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", data.info.clas.openType, R.singleton.CurrentLocale);
            _txtOwner.text = data.info.ceoNm;

            // 버튼 정리
            // TODO. 내 가입 여부에 따라 '입장' 버튼과 '가입' 버튼을 출력. 지금은 무조건 가입 버튼만 출력함.
            _btnEnter.gameObject.SetActive(false); 
            _btnJoin.gameObject.SetActive(true);
        }

    #endregion  // GPM functions


    #region event handling

        private void OnEnter() 
        {
            Debug.Log($"{TAG} | OnEnter(), TODO.진입 이벤트 추가 예정.");
        }

        private void OnJoin() 
        {
            Debug.Log($"{TAG} | OnJoin(), TODO.가입 이벤트 추가 예정.");
        }

    #endregion  // event handling
    }
}