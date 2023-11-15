/// <summary>
/// Elevaltor controller class 
/// @author         : HJ Lee
/// @last update    : 2023. 11. 15
/// @version        : 0.2
/// @update
///     v0.1 (2023. 10. 04) : v1 과 tp 에서 사용하던 항목들 정리 후 적용. 
///     v0.2 (2023. 11. 15) : last sibling 기능 추가.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using DG.Tweening;

namespace Joycollab.v2
{
    public class Elevator : MonoBehaviour, iRepositoryObserver
    {
        private const string TAG = "Elevator";
        private const string TABLE = "Word";
        private const float TRANSITION_TIME = 0.5f;

        [Header("panel")]
        [SerializeField] private RectTransform _rect;

        [Header("floor data")]
        [SerializeField] private List<FloorData> _listFloorData;

        [Header("floor list")]
        [SerializeField] private CanvasGroup _groupPanel;
        [SerializeField] private Transform _trfFloorList;
        [SerializeField] private GameObject _goFloorItem;

        [Header("room list")]
        [SerializeField] private Transform _trfRoomList;
        [SerializeField] private Button _btnRoom1;
        [SerializeField] private Button _btnRoom2;
        [SerializeField] private Button _btnRoom3;
        [SerializeField] private Button _btnRoom4;
        [SerializeField] private Button _btnRoom5;
        [SerializeField] private Button _btnRoom0;

        [Header("current info")]
        [SerializeField] private Button _btnCurrent;
        [SerializeField] private Text _txtFloor;
        [SerializeField] private LocalizeStringEvent _localeEvent; 

        private bool isOn;
        private int elevatorOpt;


    #region Unity functions

        private void Awake() 
        {
            // set button listener
            _btnCurrent.onClick.AddListener(() => {
                _rect.SetAsLastSibling();

                // Debug.Log($"{isOn} -> {!isOn}");
                if (isOn)   HideFloorList();
                else        ShowFloorList();
            });

            _btnRoom1.onClick.AddListener(() => SquareCamera.singleton.TeleportForRoom(1).Forget());
            _btnRoom2.onClick.AddListener(() => SquareCamera.singleton.TeleportForRoom(2).Forget());
            _btnRoom3.onClick.AddListener(() => SquareCamera.singleton.TeleportForRoom(3).Forget());
            _btnRoom4.onClick.AddListener(() => SquareCamera.singleton.TeleportForRoom(4).Forget());
            _btnRoom5.onClick.AddListener(() => SquareCamera.singleton.TeleportForRoom(5).Forget());
            _btnRoom0.onClick.AddListener(() => SquareCamera.singleton.TeleportForRoom(0).Forget());


            // set current info
            SetInfo(0);


            // set list 
            GameObject obj = null;
            int order = 0;
            foreach (var floor in _listFloorData)
            {
                obj = Instantiate(_goFloorItem, Vector3.zero, Quaternion.identity);
                obj.GetComponent<FloorItem>().Init(this, order, floor);
                obj.transform.SetParent(_trfFloorList, false);

                order ++;
            }


            // set local variables
            isOn = false;
            elevatorOpt = 0;


            // register event
            R.singleton.RegisterObserver(this, eStorageKey.Elevator);
        }

        private void OnDestroy() 
        {
            if (R.singleton != null) 
            {
                R.singleton.UnregisterObserver(this, eStorageKey.Elevator);
            }
        }

    #endregion  // Unity functions


        private void ShowFloorList()
        {
            _groupPanel.transform.DOLocalMoveY(-120.0f, TRANSITION_TIME);
            isOn = true;
        }

        private void HideFloorList() 
        {
            _groupPanel.transform.DOLocalMoveY(400.0f, TRANSITION_TIME);
            isOn = false;
        }

        private void SetInfo(int order) 
        {
            _txtFloor.text = _listFloorData.Count > 0 ? _listFloorData[order].FloorString : "1F";
            _localeEvent.StringReference.SetReference(TABLE, _listFloorData.Count > 0 ? _listFloorData[order].FloorName : "로비");
        }


        public void PostSelection(int order) 
        {
            _rect.SetAsLastSibling();

            if (isOn)   HideFloorList();
            else        ShowFloorList();

            SetInfo(order);
        }


    #region Event handling

        public void UpdateInfo(eStorageKey key) 
        {
            if (key == eStorageKey.Elevator) 
            {
                if (elevatorOpt != R.singleton.ElevaotrOpt) 
                {
                    elevatorOpt = R.singleton.ElevaotrOpt;

                    if (elevatorOpt == 0)   HideFloorList();
                    else                    ShowFloorList();
                }
            }
        }

    #endregion  // Event handling
    }
}