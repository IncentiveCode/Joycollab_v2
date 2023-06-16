/// <summary>
/// [mobile]
/// 채팅 화면을 제어하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 16
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 16) : 최초 생성, gpm webview 적용.
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class ChatM : FixedView
    {
        private WebviewController ctrl;

    
    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        private void OnDestroy() 
        {
            if (MobileEvents.singleton != null) 
            {
                MobileEvents.singleton.OnBackButtonProcess -= BackButtonProcess;
            }
        }

    #endregion  // Unity functions


    #region FixedView

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MobileScene_Chat;
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();
            base.Appearing();
        }

        public async override UniTaskVoid Show(string opt) 
        {
            base.Show(opt).Forget();
            await Refresh(opt);
            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();

            WebviewBuilder.singleton.RequestClear();
        }

    #endregion  // FixedView


    #region event handling

        private async UniTask<int> Refresh(string opt="") 
        {
            string token = R.singleton.accessToken;
            int workspaceSeq = R.singleton.workspaceSeq;
            int memberSeq = R.singleton.memberSeq;
            string region = R.singleton.Region;

            string url = (opt.Length == 0) ?
                string.Format(URL.MOBILE_CHAT_LINK, memberSeq, region, token) :
                string.Format(URL.MOBILE_CHATVIEW_LINK, memberSeq, opt, region, token);

            ctrl = WebviewBuilder.singleton.Build();
            ctrl.ShowMobileChat(url);

            await UniTask.Yield();
            return 0;
        }        

        private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return; 
            if (visibleState != eVisibleState.Appeared) return;

            if (WebviewBuilder.singleton.Active()) 
            {
                ctrl.GoBack();
            }
            else 
            {
                BackProcess();
            }
        }

        private void BackProcess() 
        {
            ViewManager.singleton.Pop();
        }

    #endregion  // event handling
    }
}