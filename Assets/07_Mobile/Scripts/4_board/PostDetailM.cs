/// <summary>
/// [mobile]
/// 게시물 상세 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 10
/// @version        : 0.1
/// @update
///     v0.1 (2023. 07. 10) : 최초 생성.
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{ 
	public class PostDetailM : FixedView
	{
		private const string TAG = "PostDetailM";

		[Header("module")]
		[SerializeField] private BoardModule _module;

		[Header("button")]
		[SerializeField] private Button _btnBack;
		[SerializeField] private Button _btnEdit;
		[SerializeField] private Button _btnDelete;
		[SerializeField] private Button _btnMark;

		[Header("bookmark")]
		[SerializeField] private Image _imgMark;
		[SerializeField] private Sprite _sprMark;
		[SerializeField] private Sprite _sprMarkOn;

		[Header("content")]
		[SerializeField] private Text _txtTitle;
		[SerializeField] private TMP_Text _txtCreator;
		[SerializeField] private Text _txtCreateDate;
        [SerializeField] private Text _txtAttachedFileCount;
        [SerializeField] private Text _txtViewCount;
		[SerializeField] private Text _txtDetail;

		[Header("attached file")]
		[SerializeField] private Transform _transformFile;
		[SerializeField] private GameObject _goBoardFile;

		[Header("comment")]
		[SerializeField] private Transform _transformComment;
		[SerializeField] private Text _txtCommentCount;
		[SerializeField] private GameObject _goComment;
		[SerializeField] private GameObject _goSubComment;

		[Header("write comment")]
		[SerializeField] private TMP_InputField _inputComment;
		[SerializeField] private Button _btnWriteComment;

		// local variables
		private BoardData data;
		private int seq;
		private bool isMarked;


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

	#endregion	// Unity functions


	#region FixedView functions

		protected override void Init() 
		{
			base.Init();
			viewID = ID.MobileScene_PostDetail;


			// set 'input field' listener
			SetInputFieldListener(_inputComment);
			_inputComment.onSubmit.AddListener((value) => Debug.Log($"{TAG} | write comment, {value}"));


			// set 'button' listener
			_btnBack.onClick.AddListener(() => BackProcess());
			_btnEdit.onClick.AddListener(() => ViewManager.singleton.Push(S.MobileScene_PostWrite, seq.ToString()));
			_btnDelete.onClick.AddListener(() => {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string message = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "삭제 안내", currentLocale);

				// TODO. DeleteInfo() 작성
				// PopupBuilder.singleton.OpenConfirm(message, () => DeleteInfo().Forget());
			});
			_btnMark.onClick.AddListener(() => {
				// TODO. async 로 bookmark check
			});


			// init local variables
			seq = 0;
			isMarked = false;
		}

		public async override UniTaskVoid Show(string opt) 
		{
			base.Show().Forget();

			int temp = -1;
			int.TryParse(opt, out temp);

			seq = temp;
			data = Tmp.singleton.GetBoardInfo(seq);
			if (data == null) 
			{
				// TODO. error 처리
			}

			await Refresh();
			base.Appearing();
		}

	#endregion	// FixedView functions


	#region event handling

		private async UniTask<int> Refresh() 
		{
            // view control
            ViewManager.singleton.ShowNavigation(false);

			// clear items
			ClearItems(_transformFile);
			ClearItems(_transformComment);

			// display info
			this.seq = data.info.seq;

			_txtTitle.text = data.info.title;
			_txtCreator.text = data.info.createMember.nickNm;

			// time calculate
			DateTime now = DateTime.Now;
			DateTime createDate = Convert.ToDateTime(data.info.createdDate);
			TimeSpan diff = now - createDate;

			if (diff.Days == 0) 
			{
				Locale currentLocale = LocalizationSettings.SelectedLocale;
				string temp = string.Empty;

				if (diff.Hours == 0) 
				{
					temp = LocalizationSettings.StringDatabase.GetLocalizedString("Word",
						diff.Minutes == 1 ? "시간.1분 전" : "시간.수분 전",
						currentLocale);
					_txtCreateDate.text = string.Format(temp, diff.Minutes);
				}
				else 
				{
					temp = LocalizationSettings.StringDatabase.GetLocalizedString("Word",
						diff.Hours == 1 ? "시간.1시간 전" : "시간.수시간 전",
						currentLocale);
					_txtCreateDate.text = string.Format(temp, diff.Hours);
				}
			}
			else 
			{
				_txtCreateDate.text = data.info.createdDate;
			}

			_txtAttachedFileCount.text = data.info.attachedFile.Count.ToString();
			_txtViewCount.text = data.info.readCount.ToString();
			_txtDetail.text = data.info.content;

            await UniTask.Yield();
            return 0;
		}

		private void ClearItems(Transform trf) 
		{
			var children = trf.GetComponentInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.name == trf.name) continue;
                Destroy(child.gameObject);
            }
		}

		private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return; 
            if (visibleState != eVisibleState.Appeared) return;

            if (PopupBuilder.singleton.GetPopupCount() > 0)
            {
                PopupBuilder.singleton.RequestClear();
            }
            else 
            {
                BackProcess();
            }
        }

        private void BackProcess() 
        {
            ViewManager.singleton.Pop(true);
        }

	#endregion	// event handling
	}
}
