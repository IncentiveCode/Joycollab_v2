/// <summary>
/// [mobile]
/// Alarm Item Script
/// @author         : HJ Lee
/// @last update    : 2023. 06. 14
/// @version        : 0.1
/// @update
///     v0.1 (2022. 06. 14) : 최초 생성
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using TMPro;

namespace Joycollab.v2 
{
	public class AlarmItemM : InfiniteScrollItem 
	{
		[Header("image")]
		[SerializeField] private RawImage _imgPhoto;
		[SerializeField] private Image _imgMark;

		[Header("button")]
		[SerializeField] private Button _btnItem;
		[SerializeField] private Button _btnDelete;

		[Header("text")]
		[SerializeField] private TMP_Text _txtTitle;
		[SerializeField] private TMP_Text _txtContent;
		[SerializeField] private TMP_Text _txtDate;

		// local variables
		private int seq;	
		private bool isRead;


	#region Unity functions

		private void Awake() 
		{
			_btnItem.onClick.AddListener(OnClick);
			_btnDelete.onClick.AddListener(OnDeleteClick);
			_btnDelete.gameObject.SetActive(false);
		}

	#endregion	// Unity functions


	#region GPM functions

		public override void UpdateData(InfiniteScrollData itemData) 
		{
			base.UpdateData(itemData);

			AlarmData data = (AlarmData) itemData;

			this.seq = data.info.seq;
			_imgMark.gameObject.SetActive(! data.info.read);
			_txtTitle.text = data.info.title;
			_txtDate.text = data.info.dtm;

			// set text - TODO. 권한 체크
			_txtContent.text = data.info.content;
			

			// TODO. set texture
		}

		public void OnClick() => OnSelect();

		public void OnDeleteClick() 
		{
			Debug.Log("AlarmItemM | item delete. seq : "+ seq);
		}

	#endregion	// GPM functions
	}
}