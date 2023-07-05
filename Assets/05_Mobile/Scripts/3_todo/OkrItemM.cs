/// <summary>
/// [mobile]
/// OKR Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 07. 03
/// @version        : 0.1
/// @update
///     v0.1 (2022. 07. 03) : 최초 생성
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
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
            if (! data.loadMore) 
            {
                this.seq = data.seq;
                this.isKeyResult = data.isKeyResult;
                
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
                size.y = smaller ? 100f : 142f;
                SetSize(size);
                OnUpdateItemSize();

                if (! data.isKeyResult)
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    switch (data.info.shereType - 1) 
                    {
                        case S.SHARE_DEPARTMENT :
                            string t = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "공유 (부서)", currentLocale);
                            bool isTop = string.IsNullOrEmpty(data.info.topSpace.nm);
                            _txtShareOpt.text = string.Format(t, isTop ? data.info.topSpace.nm : data.info.space.nm);
                            break;

                        case S.SHARE_COMPANY :
                            _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "공유 (전사)", currentLocale);
                            break;

                        case S.SHARE_NONE :
                        default :
                            _txtShareOpt.text = string.Empty;
                            break;
                    }
                }
                else 
                {
                    _txtShareOpt.text = string.Empty;
                }
            }
        }

        public void OnClick()
        {
            ViewManager.singleton.Push(S.MobileScene_OkrDetail, seq.ToString());
        }

        public void OnLoadMoreClick() => OnSelect();

    #endregion  // GPM functions
    }
}