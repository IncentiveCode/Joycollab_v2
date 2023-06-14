/// <summary>
/// [mobile]
/// To-Do Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 06. 14
/// @version        : 0.1
/// @update
///     v0.1 (2022. 06. 14) : 최초 생성
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class ToDoItemM : InfiniteScrollItem
    {
        [Header("texts")]
        [SerializeField] private TMP_Text _txtTitle;
        [SerializeField] private TMP_Text _txtCreator; 
        [SerializeField] private TMP_Text _txtCreateDate;
        [SerializeField] private TMP_Text _txtPeriod;
        [SerializeField] private TMP_Text _txtDoneDate;
        [SerializeField] private TMP_Text _txtDetails;

        [Header("normal buttons")]
        [SerializeField] private Button _btnItem;
        [SerializeField] private Button _btnDone;
        [SerializeField] private Image _imgCheck;


        [Header("load more")]
        [SerializeField] private Image _imgLoadMore;
        [SerializeField] private Button _btnLoadMore;

        // local variables
        private int seq;
        private bool isDone;


    #region Unity functions

        private void Awake() 
        {
            _btnItem.onClick.AddListener(OnClick);
            _btnDone.onClick.AddListener(OnDoneClick);
            _btnLoadMore.onClick.AddListener(OnLoadMoreClick);
        }

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData itemData) 
        {
            base.UpdateData(itemData);

            ToDoData data = (ToDoData) itemData; 

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
                _txtDetails.text = data.info.content;

                _imgCheck.gameObject.SetActive(isDone);
                _txtDoneDate.gameObject.SetActive(isDone);
            }
        }

        public void OnClick() => OnSelect();

        public void OnDoneClick() 
        {
            if (! isDone)
            {
                isDone = true;
                Debug.Log("ToDoItemM | item done. seq : "+ seq);
            }
            else 
            {
                isDone = false;
                Debug.Log("ToDoItemM | item done cancel. seq : "+ seq);
            }
        }

        public void OnLoadMoreClick() 
        {

        }

    #endregion  // GPM functions
    }
}