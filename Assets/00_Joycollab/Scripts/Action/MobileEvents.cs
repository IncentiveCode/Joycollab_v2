/// <summary>
/// Mobile 용 이벤트 정리 문서.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 21
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 21) : 최초 생성. 
/// </summary>

using System;

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