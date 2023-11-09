/// <summary>
/// 북마크 리스트 항목 Script
/// @author         : HJ Lee
/// @last update    : 2023. 11. 09
/// @version        : 0.1
/// @update
///     v0.1 (2023. 11. 08) : 최초 생성, v1 & mobile 의 내용 수정 & 기능 확장 후 적용.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2
{
    public class BookmarkItem : InfiniteScrollItem
    {
        private const string TAG = "BookmarkItem";

        [Header("text")]
        [SerializeField] private TMP_Text _txtTitle;
        [SerializeField] private TMP_Text _txtDesc;

        [Header("button")]
        [SerializeField] private Button _btnItem;
        [SerializeField] private Button _btnMark;

        // local variables
        private BookmarkData data;
        private int seq;
        private eBookmarkType type;
        private int targetSeq;

    
    #region Unity functions

        private void Awake() 
        {
            // set button listener
            _btnItem.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 특정 영역으로 이동. target seq : {targetSeq}");
            });
            _btnMark.onClick.AddListener(() => {
                Debug.Log($"{TAG} | 즐겨찾기 삭제. bookmark seq : {seq}");
            });
        }

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData itemData) 
        {
            base.UpdateData(itemData);

            this.data = (BookmarkData) itemData;
            this.seq = data.info.seq;
            this.type = data.info.type;
            this.targetSeq = data.info.targetSeq;
            
            // set text
            _txtTitle.text = data.info.title;
            _txtDesc.text = data.info.desc;
        }

    #endregion  // GPM functions
    }
}