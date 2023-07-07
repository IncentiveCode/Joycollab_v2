/// <summary>
/// [mobile]
/// Board Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 07. 07
/// @version        : 0.2
/// @update
///     v0.1 (2022. 06. 15) : 최초 생성
///     v0.2 (2022. 07. 07) : 기능 정리
/// </summary>
 
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class BoardItemM : InfiniteScrollItem
    {
        [Header("text")]
        [SerializeField] private Text _txtTitle;
        [SerializeField] private TMP_Text _txtCreator;
        [SerializeField] private Text _txtCreateDate;
        [SerializeField] private Text _txtAttachedFileCount;
        [SerializeField] private Text _txtViewCount;

        [Header("bookmark")]
        [SerializeField] private Image _imgBookmark;

        [Header("button")]
        [SerializeField] private Button _btnItem;

        [Header("load more")]
        [SerializeField] private Image _imgLoadMore;
        [SerializeField] private Button _btnLoadMore;

        // local variables
        private int seq;
        private bool isMarked;

        // data
        private BoardData data;

    
    #region Unity functions

        private void Awake() 
        {
            _btnItem.onClick.AddListener(OnClick);
            _btnLoadMore.onClick.AddListener(OnLoadMoreClick);
        }

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData itemData)
        {
            base.UpdateData(itemData);
            this.data = (BoardData) itemData;

            _imgLoadMore.gameObject.SetActive(data.loadMore);
            _btnLoadMore.gameObject.SetActive(data.loadMore);
            if (! data.loadMore) 
            {
                this.seq = data.info.seq;
                this.isMarked = R.singleton.Marked(eBookmarkType.Board, data.info.seq);

                // time calculate
                DateTime now = DateTime.Now;
                DateTime createDate = Convert.ToDateTime(data.info.createdDate);
                TimeSpan diff = now - createDate;

                if (diff.Days == 0) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string temp = string.Empty;

                    if (diff.Hours == 0) 
                    {
                        temp = LocalizationSettings.StringDatabase.GetLocalizedString("Texts",
                            diff.Minutes == 1 ? "게시글 1분 전" : "게시글 수분 전",
                            currentLocale);
                        _txtCreateDate.text = string.Format(temp, diff.Minutes);
                    }
                    else 
                    {
                        temp = LocalizationSettings.StringDatabase.GetLocalizedString("Texts",
                            diff.Hours == 1 ? "게시글 1시간 전" : "게시글 수시간 전",
                            currentLocale);
                        _txtCreateDate.text = string.Format(temp, diff.Hours);
                    }
                }
                else 
                {
                    _txtCreateDate.text = data.info.createdDate;
                }

                _txtTitle.text = data.info.title;
                _txtCreator.text = data.info.createMember.nickNm;
                _txtAttachedFileCount.text = string.Format("{0}", data.info.attachedFile.Count);
                _txtViewCount.text = string.Format("{0}", data.info.readCount);
            }
        }

        public void OnClick() 
        {
            ViewManager.singleton.Push(S.MobileScene_PostDetail, seq.ToString());
        }

        public void OnLoadMoreClick() => OnSelect();

    #endregion  // GPM functions
    }
}