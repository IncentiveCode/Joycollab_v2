/// <summary>
/// [PC Web]
/// 약관 상세 화면
/// @author         : HJ Lee
/// @last update    : 2023. 08. 11.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 08. 11) : v1 에서 만들었던 Terms 수정 후 적용.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class Terms : FixedView
    {
        private const string TAG = "Terms";

        [Header("contents")] 
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtContent;
        [SerializeField] private Scrollbar _scrollbar;

        [Header("Button")] 
        [SerializeField] private Button _btnBack;

        // local variables
        private Locale currentLocale;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();
        }

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
        }

        public async override UniTaskVoid Show(string opt) 
        {
            base.Show().Forget();
            await Refresh(opt);
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region event handling

        private async UniTask<int> Refresh(string opt) 
        {
            bool typeError = false;

            switch (opt) 
            {
                case S.TERMS_OF_USAGE :
                    _txtTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "이용약관", currentLocale);
                    break;

                case S.TERMS_OF_PRIVACY :
                    _txtTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "개인정보 수집 및 이용", currentLocale);
                    break;

                case S.TERMS_OF_MARKETING :
                    _txtTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "마케팅 정보 수신", currentLocale);
                    break;

                default :
                    typeError = true;
                    break;
            }

            if (typeError) 
            {
                _txtTitle.text = _txtContent.text = string.Empty;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "잘못된 접근", currentLocale),
                    () => ViewManager.singleton.Pop()
                );
                await UniTask.Yield();
            }
            else 
            {
                string url = string.Format(URL.TERMS, opt);
                PsResponse<ResTerm> res = await NetworkTask.RequestAsync<ResTerm>(url, eMethodType.GET);
                if (string.IsNullOrEmpty(res.message)) 
                {
                    _txtContent.text = res.data.content;
                    _scrollbar.value = 1;
                }
                else 
                {
                    _txtTitle.text = _txtContent.text = string.Empty;
                    PopupBuilder.singleton.OpenAlert(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "정보 갱신 실패", currentLocale),
                        () => ViewManager.singleton.Pop()
                    );
                }
            }


            // set local variables
            currentLocale = LocalizationSettings.SelectedLocale;
            return 0;
        }    

    #endregion  // event handling
    }


    [Serializable]
    public class ResTerm 
    {
        public string useYn;
        public int seq;
        public string title;
        public string content;
        public int vs;
    }
}