/// <summary>
/// 약관 동의 화면
/// @author         : HJ Lee
/// @last update    : 2023. 11. 20.
/// @version        : 0.3
/// @update
///     v0.1 (2023. 08. 11) : v1 에서 만들었던 Agreement 수정 후 적용.
///     v0.2 (2023. 08. 28) : World 에서 추가된 나이 관련 옵션 추가.
///     v0.3 (2023. 11. 20) : guest 용 agreement 추가.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class Agreement : FixedView
    {
        private const string TAG = "Agreement";

        [Header("target")]
        [SerializeField] private eSignInType type;

        [Header("Agree to All")]
        [SerializeField] private Toggle _toggleAll;

        [Header("Agree to Terms")]
        [SerializeField] private Button _btnTerms;
        [SerializeField] private Toggle _toggleTerms;

        [Header("Agree to Privacy")]
        [SerializeField] private Button _btnPrivacy;
        [SerializeField] private Toggle _togglePrivacy;

        [Header("Agree to Age limit")]
        [SerializeField] private Toggle _toggleAgeLimit;

        [Header("Agree to Marketing")]
        [SerializeField] private Button _btnMarketing;
        [SerializeField] private Toggle _toggleMarketing;

        [Header("Agree to Messaging")]
        [SerializeField] private Toggle _toggleMail;
        [SerializeField] private Toggle _toggleSms;

        [Header("Button")] 
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnNext;

        // local variables
        private bool isInvite;
        private bool isFreetrial;
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


            // check scene opt
            if (! isOffice && ! isWorld && ! isMobile) 
            {
                Debug.Log($"{TAG} | view tag 설정이 잘못 되었습니다.");
                return;
            }


            // set toggle listener
            _toggleAll.onValueChanged.AddListener((isOn) => {
                _toggleTerms.isOn = isOn;
                _togglePrivacy.isOn = isOn;
                if (isWorld)
                {
                    _toggleAgeLimit.isOn = isOn;
                }
                if (_toggleMarketing != null)
                {
                    _toggleMarketing.isOn = isOn;
                }
                _btnNext.interactable = isOn;
            });

            _toggleTerms.onValueChanged.AddListener((isOn) => CheckState());
            _togglePrivacy.onValueChanged.AddListener((isOn) => CheckState());
            if (isWorld)
            {
                _toggleAgeLimit.onValueChanged.AddListener((isOn) => CheckState());
            }
            if (_toggleMarketing != null)
            {
                _toggleMarketing.onValueChanged.AddListener((isOn) => {
                    _toggleMail.isOn = isOn;
                    _toggleMail.interactable = isOn;

                    // TODO. 나중에 SMS 관련 기능이 생기면 그 때 반영.
                    // _toggleSms.isOn = isOn;
                    // _toggleSms.interactable = isOn;
                });
            }


            // set button listener
            _btnTerms.onClick.AddListener(() => {
                SaveState(currentData);
                ViewManager.singleton.Push(
                    isWorld ? S.WorldScene_Terms : S.SignInScene_Terms,
                    S.TERMS_OF_USAGE
                );
            });
            _btnPrivacy.onClick.AddListener(() => {
                SaveState(currentData);
                ViewManager.singleton.Push(
                    isWorld ? S.WorldScene_Terms : S.SignInScene_Terms,
                    S.TERMS_OF_PRIVACY
                );
            });
            if (_btnMarketing != null) 
            {
                _btnMarketing.onClick.AddListener(() => {
                    SaveState(currentData);
                    ViewManager.singleton.Push(
                        isWorld ? S.WorldScene_Terms : S.SignInScene_Terms,
                        S.TERMS_OF_MARKETING
                    );
                });
            }
            _btnBack.onClick.AddListener(() => { 
                PlayerPrefs.DeleteKey(S.CURRENT_AGREEMENT);

                ViewManager.singleton.PopAll();
                ViewManager.singleton.Push(
                    isWorld ? S.WorldScene_SignIn : S.SignInScene_SignIn
                );
            });
            _btnNext.onClick.AddListener(Next);


            // set local variable
            currentData = new AgreementData();
            PlayerPrefs.SetString(S.CURRENT_AGREEMENT, currentData.ToJson());
        }

        public override async UniTaskVoid Show() 
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
            if (isWorld)
            {
                _toggleAgeLimit.isOn = currentData.agreeToAgeLimit;
            }
            if (_toggleMarketing != null)   
            {
                _toggleMarketing.isOn = currentData.agreeToMarketing;
            }
            if (_toggleMail != null)
            {
                _toggleMail.interactable = currentData.agreeToMarketing;;
                _toggleMail.isOn = currentData.agreeToReceiveMail;
            }
            if (_toggleSms != null)
            {
                _toggleSms.interactable = currentData.agreeToMarketing;
                _toggleSms.isOn = currentData.agreeToReceiveSMS;
            }
            CheckState();

            if (isOffice)
            {
                // set local variables
                isInvite = R.singleton.GetParam(Key.INVITED).Equals(S.TRUE);
                isFreetrial = R.singleton.GetParam(Key.FREETRIAL).Equals(S.TRUE);
                _btnBack.gameObject.SetActive(! isInvite && ! isFreetrial);
            }
            else if (isWorld) 
            {
                _btnBack.gameObject.SetActive(true);
            }

            await UniTask.Yield();
            return 0;    
        }

        private void CheckState() 
        {
            bool terms = _toggleTerms.isOn;
            bool privacy = _togglePrivacy.isOn;

            if (isOffice) 
            {
                _btnNext.interactable = terms && privacy;
            }
            else if (isWorld) 
            {
                bool ageLimit = _toggleAgeLimit.isOn;
                _btnNext.interactable = terms && privacy && ageLimit;
            }
        }

        private void SaveState(AgreementData data) 
        {
            data.agreeToAll = _toggleAll.isOn;
            data.agreeToTerms = _toggleTerms.isOn;
            data.agreeToPrivacy = _togglePrivacy.isOn;
            data.agreeToAgeLimit = isWorld ? _toggleAgeLimit.isOn : false;
            if (_toggleMarketing != null)
            {
                data.agreeToMarketing = _toggleMarketing.isOn;
            }
            data.agreeToReceiveSMS = _toggleSms.isOn;
            data.agreeToReceiveMail = _toggleMail.isOn;

            PlayerPrefs.SetString(S.CURRENT_AGREEMENT, data.ToJson());
        }

        private void Next()
        {
            if (isOffice) 
            {
                if (isInvite || isFreetrial) 
                    ViewManager.singleton.Push(S.SignInScene_Greetings);
                else 
                    ViewManager.singleton.Push(S.SignInScene_SignUp);
            }
            else if (isWorld) 
            {
                switch (type) 
                {
                    case eSignInType.Guest :
                        ViewManager.singleton.Push(S.WorldScene_Guest);
                        break;

                    case eSignInType.Invite :
                        Debug.Log($"{TAG} | TODO. 초대 받은 사용자 처리.");
                        break;

                    case eSignInType.Member :
                    default :
                        ViewManager.singleton.Push(S.WorldScene_SignUp);
                        break;
                }
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
        public bool agreeToAgeLimit;
        public bool agreeToMarketing;
        public bool agreeToReceiveSMS;
        public bool agreeToReceiveMail;

        public AgreementData() => Init();

        public void Init() 
        {
            agreeToAll = agreeToTerms = agreeToPrivacy = agreeToAgeLimit = false; 
            agreeToMarketing = agreeToReceiveSMS = agreeToReceiveMail = false;
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