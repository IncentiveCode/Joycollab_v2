/// <summary>
/// [PC Web]
/// 여기저기 떨어져 있는 팝업 생성 함수를 하나로 묶기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 02. 09
/// @version        : 0.1
/// @update
///     - v0.1 : TP 에서 작업했던 내용을 가지고 와서 편집. (작업중)
/// </summary>


using UnityEngine;

namespace Joycollab.v2
{
    public class PopupBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject _goPopup;
        [SerializeField] private Transform _transform;

        public static PopupBuilder Instance;


    #region Unity functions
        private void Awake() 
        {
            Instance = this;

            if (_transform == null) 
            {
                _transform = GameObject.Find(S.POPUP_CANVAS).GetComponent<Transform>();
            }
        }
    #endregion


    #region Public function

    #endregion
    }
}