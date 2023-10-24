/// <summary>
/// floor item controller class 
/// @author         : HJ Lee
/// @last update    : 2023. 10. 04
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 04) : v1 과 tp 에서 사용하던 항목들 정리 후 적용. 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

namespace Joycollab.v2
{
    public class FloorItem : MonoBehaviour
    {
        private const string TAG = "FloorItem";
        private const string TABLE = "Word";

        [Header("Elevator instance")]
        [SerializeField] private Elevator _elevator;

        [Header("Floor info")]
        [SerializeField] private Text _txtFloorNo;
        [SerializeField] private LocalizeStringEvent _localeEvent; 

        private Button btn;
        private int order;
        private int floorNo;
    

        public void Init(Elevator ev, int order, FloorData data) 
        {
            _elevator = ev;
            this.order = order;
            floorNo = data.FloorNo;

            _txtFloorNo.text = data.FloorString;
            _txtFloorNo.gameObject.SetActive(data.FloorNo != 0);
            _localeEvent.StringReference.SetReference(TABLE, data.FloorName);

            // set button listener
            btn = GetComponent<Button>();
            btn.onClick.AddListener(() => {
                if (floorNo == 0) 
                {
                    var manager = WorldNetworkManager.singleton;
                    manager.StopClient();
                    manager.StopServer();
                }
                else 
                {
                    // Debug.Log($"{TAG} | teleport, "+ floorNo);
                    SquareCamera.singleton.Teleport(floorNo).Forget();
                    _elevator.PostSelection(order);
                }
            });
        } 
    }
}