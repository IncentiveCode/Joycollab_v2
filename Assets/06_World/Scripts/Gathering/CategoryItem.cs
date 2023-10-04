/// <summary>
/// [world]
/// 모임방 리스트 > 카테고리 항목 Script
/// @author         : HJ Lee
/// @last update    : 2023. 10. 04
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 04) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;

namespace Joycollab.v2
{
    public class CategoryItem : InfiniteScrollItem
    {
        private const string TAG = "CategoryItem";

        [Header("text")]
        [SerializeField] private Toggle _toggle;
        [SerializeField] private Text _txtName;

        // local variables
        private RoomCategoryData data;
        private ToggleGroup group;

    
    #region GPM functions

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            data = (RoomCategoryData) scrollData;

            // 문구 정리
            _txtName.text = data.info.nm;

            // toggle group 정리
            if (_toggle.group == null) 
            {
                if (transform.parent.TryGetComponent<ToggleGroup>(out group)) 
                {
                    _toggle.group = group;
                }
                else 
                {
                    Debug.Log($"{TAG} | Toggle Group 을 지정할 수 없음.");
                }
            }
        }

    #endregion  // GPM functions
    }
}