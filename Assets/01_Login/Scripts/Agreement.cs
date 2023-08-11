/// <summary>
/// [PC Web]
/// 약관 동의 화면
/// @author         : HJ Lee
/// @last update    : 2023. 08. 11.
/// @version        : 0.1
/// @update
///     v0.1 : v1 에서 만들었던 Agreement 수정 후 적용.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class Agreement : FixedView
    {
        private const string TAG = "Agreement";

        [Header("Agree to All")]
        [SerializeField] private Toggle _toggleAll;

        [Header("Agree to Terms")]
        [SerializeField] private Button _btnTerms;
        [SerializeField] private Toggle _toggleTerms;

        [Header("Agree to Privacy")]
        [SerializeField] private Button _btnPrivacy;
        [SerializeField] private Toggle _togglePrivacy;

        [Header("Agree to Marketing")]
        [SerializeField] private Button _btnMarketing;
        [SerializeField] private Toggle _toggleMarketing;

        [Header("Agree to Messaging")]
        [SerializeField] private Toggle _toggleMail;
        [SerializeField] private Toggle _toggleSms;

        [Header("Button")] 
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnNext;

        [Header("tag")]
        [TagSelector]
        [SerializeField] private string viewTag;

        // local variables
        private bool isWorld;
        private bool isInvite;
        private bool isFreetrial;
        private Locale currentLocale;
        private AgreementData currentData;


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


            // set toggle listener
            _toggleAll.onValueChanged.AddListener((isOn) => {
                _toggleTerms.isOn = isOn;
                _togglePrivacy.isOn = isOn;
                _toggleMarketing.isOn = isOn;
                _btnNext.interactable = isOn;
            });

            _toggleTerms.onValueChanged.AddListener((isOn) => CheckState());
            _togglePrivacy.onValueChanged.AddListener((isOn) => CheckState());
            _toggleMarketing.onValueChanged.AddListener((isOn) => {
                _toggleMail.isOn = isOn;
                _toggleMail.interactable = isOn;

                // TODO. 나중에 SMS 관련 기능이 생기면 그 때 반영.
                // _toggleSms.isOn = isOn;
                // _toggleSms.interactable = isOn;
            });


            // set button listener
            _btnTerms.onClick.AddListener(() => {
                SaveState(currentData);
                ViewManager.singleton.Push(
                    isWorld ? S.WorldScene_Terms : S.LoginScene_Terms,
                    S.TERMS_OF_USAGE
                );
            });
            _btnPrivacy.onClick.AddListener(() => {
                SaveState(currentData);
                ViewManager.singleton.Push(
                    isWorld ? S.WorldScene_Terms : S.LoginScene_Terms,
                    S.TERMS_OF_PRIVACY
                );
            });
            _btnMarketing.onClick.AddListener(() => {
                SaveState(currentData);
                ViewManager.singleton.Push(
                    isWorld ? S.WorldScene_Terms : S.LoginScene_Terms,
                    S.TERMS_OF_MARKETING
                );
            });
            _btnBack.onClick.AddListener(() => { 
                PlayerPrefs.DeleteKey(S.CURRENT_AGREEMENT);

                ViewManager.singleton.PopAll();
                ViewManager.singleton.Push(
                    isWorld ? S.WorldScene_SignIn : S.LoginScene_Login
                );
            });
            _btnNext.onClick.AddListener(() => Next());


            // set local variable
            currentData = new AgreementData();
            PlayerPrefs.SetString(S.CURRENT_AGREEMENT, currentData.ToJson());
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region event handling 

        private async UniTask<int> Refresh() 
        {
            if (PlayerPrefs.HasKey(S.CURRENT_AGREEMENT))
            {
                string temp = PlayerPrefs.GetString(S.CURRENT_AGREEMENT, string.Empty);
                currentData.LoadFromJson(temp);
            }
            else 
            {
                currentData.Init();
            }

            _toggleAll.isOn = currentData.agreeToAll;
            _toggleTerms.isOn = currentData.agreeToTerms;
            _togglePrivacy.isOn = currentData.agreeToPrivacy;
            _toggleMarketing.isOn = currentData.agreeToMarketing;
            _toggleMail.interactable = currentData.agreeToMarketing;;
            _toggleMail.isOn = currentData.agreeToReceiveMail;
            _toggleSms.interactable = currentData.agreeToMarketing;
            _toggleSms.isOn = currentData.agreeToReceiveSMS;
            CheckState();


            // world 의 경우, 뒤로가기 버튼 오픈.
            isWorld = viewTag.Equals(S.WorldScene_ViewTag);
            if (isWorld) 
            {
                _btnBack.gameObject.SetActive(true);
            }
            // invite 의 경우, 하단 로그인 버튼 가림.
            else
            {
                isInvite = R.singleton.GetParam(Key.INVITED).Equals(S.TRUE);
                isFreetrial = R.singleton.GetParam(Key.FREETRIAL).Equals(S.TRUE);
                _btnBack.gameObject.SetActive(! isInvite && ! isFreetrial);
            }


            // set local variables
            currentLocale = LocalizationSettings.SelectedLocale;

            await UniTask.Yield();
            return 0;    
        }

        private void CheckState() 
        {
            bool terms = _toggleTerms.isOn;
            bool privacy = _togglePrivacy.isOn;
            _btnNext.interactable = (terms && privacy) ? true : false;
        }

        private void SaveState(AgreementData data) 
        {
            data.agreeToAll = _toggleAll.isOn;
            data.agreeToTerms = _toggleTerms.isOn;
            data.agreeToPrivacy = _togglePrivacy.isOn;
            data.agreeToMarketing = _toggleMarketing.isOn;
            data.agreeToReceiveSMS = _toggleSms.isOn;
            data.agreeToReceiveMail = _toggleMail.isOn;

            PlayerPrefs.SetString(S.CURRENT_AGREEMENT, currentData.ToJson());
        }

        private void Next()
        {
            if (isWorld) 
            {
                ViewManager.singleton.Push(S.WorldScene_Join);
            }
            else 
            {
                if (isInvite || isFreetrial) 
                    ViewManager.singleton.Push(S.LoginScene_Greetings);
                else 
                    ViewManager.singleton.Push(S.LoginScene_Join);
            }
        }

    #endregion  // event handling 
    }


    [Serializable]
    public class AgreementData 
    {
        public bool agreeToAll;
        public bool agreeToTerms;
        public bool agreeToPrivacy;
        public bool agreeToMarketing;
        public bool agreeToReceiveSMS;
        public bool agreeToReceiveMail;

        public AgreementData() => Init();

        public void Init() 
        {
            agreeToAll = agreeToTerms = agreeToPrivacy = agreeToMarketing = agreeToReceiveSMS = agreeToReceiveMail = false;
        }

        public string ToJson() 
        {
            // Debug.Log("[TEST] save | data : "+ JsonUtility.ToJson(this));
            return JsonUtility.ToJson(this);
        }

        public void LoadFromJson(string json) 
        {
            // Debug.Log("[TEST] load | data : "+ json);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}