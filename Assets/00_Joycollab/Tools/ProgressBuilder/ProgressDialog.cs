/// <summary>
/// 프로그레스 다이얼로그 제어 스크립트
/// @author         : HJ Lee
/// @last update    : 2023. 03. 07 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 07) : 최초 생성
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class ProgressDialog : MonoBehaviour
    {
        private float fAutoCloseTime;
        private float fTimer;
        private System.Action func;


    #region Unity functions

        private void Update() 
        {
            if (fAutoCloseTime == 0f) return;

            fTimer += Time.deltaTime;
            if (fTimer >= fAutoCloseTime)
            {
                Close();
            }
        }

    #endregion  // Unity functions


    #region Public functions

        public void Open(float timer=0f, System.Action func=null)
        {
            this.fAutoCloseTime = timer;
            this.func = func;
            fTimer = 0f;
        }

        public void ForceClose() => Destroy(gameObject);

    #endregion  // Public functions


    #region Private functions
    
        private void Close() 
        {
            func?.Invoke();
            Destroy(gameObject);
        }

    #endregion
    }
}