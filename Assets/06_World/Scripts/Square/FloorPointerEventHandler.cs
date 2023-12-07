/// <summary>
/// 커뮤니티 센터, 모임방 등에서 마우스 클릭을 관리하는 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 10. 25
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 25) : 최초 작성.
/// </summary>

using UnityEngine;
using Mirror;

namespace Joycollab.v2
{
    public class FloorPointerEventHandler : MonoBehaviour
    {
        private const string TAG = "FloorPointerEventHandler";
        private bool isOver;
        private Vector3 worldPosition;
        private Camera cam;
        

    #region Unity functions 

        private void Awake() 
        {
            worldPosition = Vector3.zero;
        }

        private void Update() 
        {
            if (isOver && Input.GetMouseButtonUp(0))
            {
                worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log($"{TAG} | click position : {worldPosition}");

                // CommandFly(worldPosition);
            }
        }

        private void OnMouseOver() 
        {
            isOver = true;     
        }

        private void OnMouseExit() 
        {
            isOver = false;     
        }

    #endregion  // Unity functions 


    #region Command function

        private void CommandFly(Vector3 position, NetworkConnectionToClient sender = null) 
        {
            if (! sender.identity.TryGetComponent(out WorldAvatar avatar)) 
            {
                Debug.Log($"{TAG} | WorldAvatar component 가 없음.");
            }
            else 
            {
                Debug.Log($"{TAG} | call CommandFly()");
                avatar.UpdateAVatarPosition(position);
            }

            if (! sender.identity.TryGetComponent(out WorldPlayer player)) 
            {
                Debug.Log($"{TAG} | WorldPlayer component 가 없음.");  
            }
            else 
            {
                Debug.Log($"{TAG} | call CommandFly()");
                player.UpdateAVatarPosition(position);
            }
        }

    #endregion  // Command function
    }
}