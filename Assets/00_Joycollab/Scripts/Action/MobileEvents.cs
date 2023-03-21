/// <summary>
/// Mobile 용 이벤트 정리 문서.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 21
/// @version        : 1.0
/// @update
///     v1.0 (2023. 03. 21) : 최초 생성. 
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2
{
    public class MobileEvents : Singleton<MobileEvents>
    {
        public event Action<string> OnBackButtonProcess;
        public void BackButtonProcess(string name) 
        {
            if (OnBackButtonProcess != null) 
            {
                OnBackButtonProcess(name);
            }
        }
    }
}