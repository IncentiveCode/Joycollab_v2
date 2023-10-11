/// <summary>
/// [mobile]
/// OKR Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 07. 26
/// @version        : 0.2
/// @update
///     v0.1 (2023. 07. 03) : 최초 생성
///     v0.2 (2023. 07. 26) : detail 로 넘어갈 때, string 이 아니라 int 형 인자를 넘기도록 수정. 
///                           O 또는 KR 일 때, item 크기 조절해서 출력.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class OkrItemM : InfiniteScrollItem
    {
        private const string TAG = "OkrItemM";

        [Header("module")]
        [SerializeField] private ToDoModule _module;

        [Header("texts")]
        [SerializeField] private TMP_Text _txtTitle;
        [SerializeField] private TMP_Text _txtCreator; 
        [SerializeField] private TMP_Text _txtPeriod;
        [SerializeField] private TMP_Text _txtShareOpt;
        [SerializeField] private TMP_Text _txtDetail;

        [Header("sprite")]
        [SerializeField] private Image _imgReply;
        [SerializeField] private Image _imgIcon;
        [SerializeField] private Sprite _sprObjective;
        [SerializeField] private Sprite _sprKeyResult;

        [Header("normal buttons")]
        [SerializeField] private Button _btnItem;
        [SerializeField] private GameObject _goDetailArea;

        [Header("load more")]
        [SerializeField] private Image _imgLoadMore;
        [SerializeField] private Button _btnLoadMore;

        // local variables
        private RectTransform rect;
        private int seq;
        private bool isKeyResult;
        private bool isLoadMore;

        // data
        private OkrData data;


    #region Unity functions

        private void Awake() 
        {
            rect = GetComponent<RectTransform>();

            _btnItem.onClick.AddListener(OnClick);
            _btnLoadMore.onClick.AddListener(OnLoadMoreClick);
        }

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData itemData) 
        {
            base.UpdateData(itemData);  
            this.data = (OkrData) itemData; 

            _imgLoadMore.gameObject.SetActive(data.loadMore);
            _btnLoadMore.gameObject.SetActive(data.loadMore);

            _txtTitle.gameObject.SetActive(! data.loadMore);
            _txtCreator.gameObject.SetActive(! data.loadMore);
            _txtPeriod.gameObject.SetActive(! data.loadMore);
            _txtShareOpt.gameObject.SetActive(! data.loadMore);
            _goDetailArea.gameObject.SetActive(! data.loadMore);

            if (data.loadMore)
            {
                Vector2 size = rect.sizeDelta;
                size.y = 70f; 
                SetSize(size);
                OnUpdateItemSize();

                this.seq = 0;
                this.isKeyResult = false;
                this.isLoadMore = true;
            }
            else
            {
                this.seq = data.seq;
                this.isKeyResult = data.isKeyResult;
                this.isLoadMore = false;
                
                // icon 처리
                _imgReply.gameObject.SetActive(isKeyResult);
                _imgIcon.sprite = isKeyResult ? _sprKeyResult : _sprObjective;

                // text 처리
                bool sub = (! this.data.isShare && this.data.isKeyResult);
                _txtTitle.text = sub ? data.subInfo.title : data.info.title;
                _txtCreator.text = sub ? data.subInfo.createMember.nickNm : data.info.createMember.nickNm;
                _txtPeriod.text = sub ? string.Format("{0} - {1}", data.subInfo.sd, data.subInfo.ed) :
                                        string.Format("{0} - {1}", data.info.sd, data.info.ed);
                _txtDetail.text = sub ? data.subInfo.content : data.info.content;

                bool smaller = string.IsNullOrEmpty(_txtDetail.text);
                _goDetailArea.SetActive(! smaller);

                Vector2 size = rect.sizeDelta;

                if (! data.isKeyResult)
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    switch (data.info.shereType - 1) 
                    {
                        case S.SHARE_DEPARTMENT :
                            bool isTop = string.IsNullOrEmpty(data.info.topSpace.nm);
                            _txtShareOpt.text = isTop ? data.info.topSpace.nm : data.info.space.nm;
                            break;

                        case S.SHARE_COMPANY :
                            _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "전사", currentLocale);
                            break;

                        case S.SHARE_NONE :
                        default :
                            _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "없음", currentLocale);
                            break;
                    }

                    size.y = smaller ? 100f : 142f;
                }
                else 
                {
                    _txtShareOpt.gameObject.SetActive(false);
                    size.y = smaller ? 80f : 122f;
                }

                SetSize(size);
                OnUpdateItemSize();
            }
        }

        public void OnClick()
        {
            if (isLoadMore) return;

            ViewManager.singleton.Push(S.MobileScene_OkrDetail, seq);
        }

        public void OnLoadMoreClick() => OnSelect();

    #endregion  // GPM functions
    }
}