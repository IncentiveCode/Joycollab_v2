/// <summary>
/// [PC Web]
/// 사용자 Login 화면
/// @author         : HJ Lee
/// @last update    : 2023. 04. 22
/// @version        : 0.8
/// @update
///     v0.1 : UI Canvas 최적화 (static canvas, active canvas 분리)
///     v0.2 : Tab key 로 input field 이동할 수 있게 수정.
///     v0.3 (2023. 02. 13) : Unitask 적용.
///     v0.4 (2023. 02. 15) : world 연결 버튼 추가. (개발 서버에만 적용).
///     v0.5 (2023. 03. 06) : 수정한 jslib 적용
///     v0.6 (2023. 04. 04) : Joycollab.v2 package 적용. R class 실험. 
///                 Strings class 에 몰아넣은 문자열들도 필요한 부분에서 사용할 수 있도록 분리. (S, Key, NetworkTask class, and etc) 
///     v0.7 (2023. 04. 14) : Popup Builder 적용
///     v0.8 (2023. 04. 22) : FixedView 적용
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class Login : FixedView
    {

    #region Unity functions
        private void Awake() 
        {

        }

        private void OnDestroy() 
        {

        }
    #endregion  // Unity functions


    #region FixedView functions
        protected override void Init() 
        {

        }

        public async override UniTaskVoid Show() 
        {

        }
    #endregion  // FixedView functions


    #region event handling
        private async UniTask<int> Refresh() 
        {

            await UniTask.Yield();
            return 0;    
        }
    #endregion  // event handling
    }
}