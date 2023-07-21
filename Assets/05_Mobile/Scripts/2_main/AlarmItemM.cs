/// <summary>
/// [mobile]
/// Alarm Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 07. 21
/// @version        : 0.3
/// @update
///     v0.1 (2023. 06. 14) : 최초 생성
///     v0.2 (2023. 07. 19) : Addressable asset 의 thumbnail 불러오는 기능 추가. delete button 삭제. content 내용 정리.
///     v0.3 (2023. 07. 21) : 버튼 이벤트 수정. 이미지 로드 수정. 
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2 
{
	public class AlarmItemM : InfiniteScrollItem 
	{
		private const string TAG = "AlarmItemM";

		[Header("image")]
		[SerializeField] private RawImage _imgPhoto;
		[SerializeField] private Image _imgMark;

		[Header("button")]
		[SerializeField] private Button _btnItem;
		[SerializeField] private Button _btnMenu;

		[Header("text")]
		[SerializeField] private TMP_Text _txtTitle;
		[SerializeField] private TMP_Text _txtName;
		[SerializeField] private TMP_Text _txtContent;
		[SerializeField] private TMP_Text _txtDate;

		[Header("for texture")]
		[SerializeField] private Vector2 _v2PhotoSize;
		[SerializeField] private Texture2D _texDefault;
		[SerializeField] private Texture2D _texSeminar;

		// local variables
		private AlarmData data;
		private int seq;	
		private string id;
		private string fileName;

		private RectTransform rectPhoto;
		private XmppTaskInfo task;
	    private AsyncOperationHandle<Texture2D> operationHandle;


	#region Unity functions

		private void Awake() 
		{
			rectPhoto = _imgPhoto.GetComponent<RectTransform>();

			_btnItem.onClick.AddListener(OnSelect);
			_btnMenu.onClick.AddListener(OnMenu);
		}

	#endregion	// Unity functions


	#region GPM functions

		public override void UpdateData(InfiniteScrollData itemData) 
		{
			base.UpdateData(itemData);

			data = (AlarmData) itemData;
			this.seq = data.info.seq;
			this.id = data.info.tp.id;
			bool isArrange = false;

            Locale currentLocale = LocalizationSettings.SelectedLocale;

			// 기본 정보 정리
			_imgMark.gameObject.SetActive(! data.info.read);

			if (id.Equals("일감"))	_txtTitle.text = R.singleton.isKorean ? "일감" : "Task";
			else					_txtTitle.text = data.info.title;

			_txtName.text = data.info.sender;

			// 시간값 정리
	        DateTime now = DateTime.Now;
			DateTime createDate = Convert.ToDateTime(data.info.dtm);
			TimeSpan diff = now - createDate;
			if (diff.Days == 0)
			{
				string t = string.Empty;
				if (diff.Hours == 0) 
				{
					t = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", 
						diff.Minutes == 1 ? "게시글 1분 전" : "게시글 수분 전", 
						currentLocale);
					_txtDate.text = string.Format(t, diff.Minutes);
				}
				else
				{
					t = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", 
						diff.Hours == 1 ? "게시글 1시간 전" : "게시글 수시간 전", 
						currentLocale);
					_txtDate.text = string.Format(t, diff.Hours);
				}
			}
			else 
			{
				_txtDate.text = data.info.dtm;
			}

			// content 정리
			switch (id) 
			{
				case S.ALARM_RESERVE_MEETING :
				case S.ALARM_UPDATE_MEETING :
				case S.ALARM_DELETE_MEETING :
				case S.ALARM_INVITE_MEETING :
				case S.ALARM_INVITE_MEETING_CANCEL :
				case S.ALARM_START_MEETING :
				case S.ALARM_DONE_MEETING :

				case S.ALARM_RESERVE_SEMINAR :
				case S.ALARM_UPDATE_SEMINAR :
				case S.ALARM_DELETE_SEMINAR :

				case S.ALARM_VOICE_CALL :
				case S.ALARM_REJECT_CALL :
				case S.ALARM_TO_DO :
					_txtContent.text = data.info.content;
					break;

				case S.ALARM_UPDATE_MEMBER :
                    _txtContent.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Member.정보 변경 안내", currentLocale);
					break;

				case S.ALARM_UPDATE_SPACE :
					isArrange = true;
                    _txtContent.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Space.정보 변경 안내", currentLocale);
					break;

				case S.ALARM_UPDATE_SEAT :
                    _txtContent.text = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Seat.정보 변경 안내", currentLocale);
					break;
				
				case S.ALARM_TASK :
					task = JsonUtility.FromJson<XmppTaskInfo>(data.info.contentJson);
	                string content = data.info.content.Replace(XmppManager.CONTENT_SPLITTER, "|");
					var arrContent = content.Split('|');
					string c0 = arrContent[0].Replace(XmppManager.TASK_SPLITTER, "|");
                	var arr = c0.Split('|'); 

					string res, key, value;
					if (arr.Length > 1) 
					{
						key = "Kanban."+ arr[0];
						value = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", key, currentLocale);
						res = string.Format(value, arr[1]); 
					}
					else
					{
						key = "Kanban."+ content;
						res = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", key, currentLocale);
					}

					if (arrContent.Length > 1) 
					{
						res += " ";

						value = LocalizationSettings.StringDatabase.GetLocalizedString("Alarm", "Kanban.여러 항목 수정", currentLocale);
						res += string.Format(value, (arrContent.Length - 1)); 
					}
					_txtContent.text = res;
					break;
				
				default :
					Debug.LogWarning($"{TAG} | 알 수 없는 알림 id : {data.info.tp.id}");
					_txtContent.text = string.Empty;
					break;
			}
			

			// texture 설정
			if (string.IsNullOrEmpty(data.info.img)) 
			{
				_imgPhoto.texture = _texDefault;
				Util.ResizeRawImage(rectPhoto, _imgPhoto, _v2PhotoSize.x, _v2PhotoSize.y);
			}
			else if (isArrange) 
			{
                string[] split  = data.info.img.Split('/');
                fileName = split[split.Length - 1];

                // HJ Lee. 2023. 01. 10. streamingAssets 을 사용하는 경우, read/write enable 을 사용할 수 없어서 문제가 발생함. addressable 도입해봄.
                Addressables.LoadAssetAsync<Texture2D>(fileName).Completed += OnLoadDone;
			}
			else 
			{
				string photoUrl = string.Format("{0}{1}", URL.SERVER_PATH, data.info.img);
                Debug.Log($"{TAG} | item get photo - url : {photoUrl}");

                if (photoUrl.Contains(S.SEMINAR))
                {
                    _imgPhoto.texture = _texSeminar;
                    Util.ResizeRawImage(rectPhoto, _imgPhoto, _v2PhotoSize.x, _v2PhotoSize.y);
                }
                else
                {
					GetTexture(photoUrl).Forget();
                }
			}
		}

	#endregion	// GPM functions


	#region image loading, addressable, button click listener

		private async UniTaskVoid GetTexture(string path)
		{
			if (_imgPhoto == null) return;

			Texture2D res = await NetworkTask.GetTextureAsync(path);
			if (res == null) 
			{
				if (_imgPhoto != null) _imgPhoto.texture = _texDefault;
			}
			else 
			{
				res.hideFlags = HideFlags.HideAndDontSave;
				res.filterMode = FilterMode.Point;
				res.Apply();				

				if (_imgPhoto != null) _imgPhoto.texture = res;
			}

			if (_imgPhoto != null)
            	Util.ResizeRawImage(rectPhoto, _imgPhoto, _v2PhotoSize.x, _v2PhotoSize.y);
		}


		private void OnLoadDone(AsyncOperationHandle<Texture2D> obj) 
		{
			switch (obj.Status) 
			{
				case AsyncOperationStatus.Succeeded :
					obj.Result.hideFlags = HideFlags.HideAndDontSave;
					obj.Result.filterMode = FilterMode.Point;
					obj.Result.Apply();

					_imgPhoto.texture = obj.Result;
					Util.ResizeRawImage(rectPhoto, _imgPhoto, _v2PhotoSize.x, _v2PhotoSize.y);

					operationHandle = obj; 
					break;

				case AsyncOperationStatus.Failed :
					Debug.Log($"{TAG} | Addressable texture load fail. image : {fileName}");
					break;

				default :
					break;
			}
		} 

		private void OnMenu() 
		{
            Locale currentLocale = LocalizationSettings.SelectedLocale;
			string title = LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "알림 관리", currentLocale);

			int targetSeq = 0;
			switch (id) 
			{
				case S.ALARM_RESERVE_MEETING :
				case S.ALARM_UPDATE_MEETING :
				case S.ALARM_DELETE_MEETING :
				case S.ALARM_INVITE_MEETING :
				case S.ALARM_INVITE_MEETING_CANCEL :
				case S.ALARM_START_MEETING :
				case S.ALARM_DONE_MEETING :

				case S.ALARM_RESERVE_SEMINAR :
				case S.ALARM_UPDATE_SEMINAR :
				case S.ALARM_DELETE_SEMINAR :

				case S.ALARM_VOICE_CALL :
				case S.ALARM_REJECT_CALL :
				case S.ALARM_TO_DO :
					int.TryParse(data.info.contentJson, out targetSeq);
					break;

				case S.ALARM_UPDATE_MEMBER :
				case S.ALARM_UPDATE_SPACE :
				case S.ALARM_UPDATE_SEAT :

				case S.ALARM_TASK : 
				default :
					targetSeq = 0;
					break;
			}

			PopupBuilder.singleton.OpenSlide(title, 
				new string[] {
					LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "알림 상세 확인", currentLocale),
					LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "알림 읽음으로 표시", currentLocale),
					LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "알림 삭제", currentLocale)
				}, 
				new string[] {	// alarm seq, tp id, target seq
					seq.ToString(),
					data.info.tp.id,
					targetSeq.ToString()
			 	}, ID.MobileScene_Alarm, true
			);
		}

	#endregion	// addressable and button click listener
	}
}