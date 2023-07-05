/// <summary>
/// [mobile]
/// To-Do Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 06. 14
/// @version        : 0.1
/// @update
///     v0.1 (2022. 06. 14) : 최초 생성
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
    public class ToDoItemM : InfiniteScrollItem
    {
        [Header("module")]
        [SerializeField] private ToDoModule _module;

        [Header("texts")]
        [SerializeField] private TMP_Text _txtTitle;
        [SerializeField] private TMP_Text _txtCreator; 
        [SerializeField] private TMP_Text _txtCreateDate;
        [SerializeField] private TMP_Text _txtPeriod;
        [SerializeField] private TMP_Text _txtDoneDate;
        [SerializeField] private TMP_Text _txtShareOpt;
        [SerializeField] private TMP_Text _txtDetail;

        [Header("normal buttons")]
        [SerializeField] private Button _btnItem;
        [SerializeField] private Button _btnDone;
        [SerializeField] private Image _imgCheck;
        [SerializeField] private GameObject _goDetailArea;

        [Header("load more")]
        [SerializeField] private Image _imgLoadMore;
        [SerializeField] private Button _btnLoadMore;

        // local variables
        private RectTransform rect;
        private int seq;
        private bool isDone;

        // data
        private ToDoData data;


    #region Unity functions

        private void Awake() 
        {
            rect = GetComponent<RectTransform>();

            _btnItem.onClick.AddListener(OnClick);
            _btnDone.onClick.AddListener(() => OnDoneClick().Forget());
            _btnLoadMore.onClick.AddListener(OnLoadMoreClick);
        }

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData itemData) 
        {
            base.UpdateData(itemData);  
            this.data = (ToDoData) itemData; 

            _imgLoadMore.gameObject.SetActive(data.loadMore);
            _btnLoadMore.gameObject.SetActive(data.loadMore);
            if (! data.loadMore) 
            {
                this.seq = data.info.seq;
                this.isDone = data.info.completeYn.Equals("Y");

                _txtTitle.text = data.info.title;
                _txtCreator.text = string.Format("{0} ({1})", data.info.createMember.nickNm, data.info.space.nm);
                _txtCreateDate.text = data.info.createdDate;
                _txtPeriod.text = string.Format("{0} - {1}", data.info.sd, data.info.ed);
                _txtDoneDate.text = data.info.completeTime;
                _txtDetail.text = data.info.content;

                bool smaller = string.IsNullOrEmpty(data.info.content);
                _goDetailArea.SetActive(! smaller);

                Vector2 size = rect.sizeDelta;
                size.y = smaller ? 100f : 142f;
                SetSize(size);
                OnUpdateItemSize();

                Locale currentLocale = LocalizationSettings.SelectedLocale;
                switch (data.info.shereType) 
                {
                    case S.SHARE_DEPARTMENT :
                        string t = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "공유 (부서)", currentLocale);
                        _txtShareOpt.text = string.Format(t, data.info.space.nm);
                        break;

                    case S.SHARE_COMPANY :
                        _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "공유 (전사)", currentLocale);
                        break;

                    case S.SHARE_NONE :
                    default :
                        _txtShareOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "공유 (개인)", currentLocale);
                        break;
                }

                // 내 정보인 경우에만 버튼 출력
                bool isMyInfo = (R.singleton.memberSeq == data.info.createMember.seq);
                _btnDone.interactable = isMyInfo;

                // 완료 마크 처리
                Color tempColor = _imgCheck.color;
                tempColor.a = isMyInfo ? 1f : 0.5f;
                _imgCheck.color = tempColor;

                // 완료 처리 
                DoneProcess(isDone);
            }
        }

        public void OnClick()
        {
            ViewManager.singleton.Push(S.MobileScene_ToDoDetail, seq.ToString());
        } 

        public async UniTaskVoid OnDoneClick() 
        {
            PsResponse<string> res = await _module.CheckItem(this.seq);
            if (string.IsNullOrEmpty(res.message)) 
            {
                isDone = !isDone;
                DoneProcess(isDone);
            }
            else 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
            }
        }

        public void OnLoadMoreClick() => OnSelect();

    #endregion  // GPM functions

        private void DoneProcess(bool done) 
        {
            _imgCheck.gameObject.SetActive(done);
            _txtDoneDate.gameObject.SetActive(done);
            _txtTitle.fontStyle = done ? FontStyles.Strikethrough : FontStyles.Normal;
            _txtPeriod.fontStyle = done ? FontStyles.Strikethrough : FontStyles.Normal;

            data.info.completeYn = done ? "Y" : "N";
            data.info.completeTime = done ? DateTime.Now.ToString("yyyy-MM-dd HH:mm") : string.Empty;
            _txtDoneDate.text = data.info.completeTime;
            
            R.singleton.AddToDoInfo(this.seq, data);
            base.UpdateData((InfiniteScrollData) data);
        }
    }
}