/// <summary>
/// [mobile]
/// 파일 아이템을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 19
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 19) : 최초 생성.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class FileItemM : InfiniteScrollItem
    {
        private const string TAG = "FileItemM";
        
        [Header("sprite")]
        [SerializeField] private Image _imgIcon;
        [SerializeField] private Sprite _sprFolder;
        [SerializeField] private Sprite _sprFile;

        [Header("text")]
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtSize;
        [SerializeField] private TMP_Text _txtLastUpdate;

        [Header("button")]
        [SerializeField] private Button _btnItem;
        [SerializeField] private Button _btnMenu;

        // local variables
        private int seq;
        private bool isFile;
        private string fileName;
        private int spaceSeq;
        private string fullPath;


    #region Unity functions

        private void Awake() 
        {
            _btnItem.onClick.AddListener(OnClick);
            _btnMenu.onClick.AddListener(OnMenuClick);
        }

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData itemData) 
        {
            base.UpdateData(itemData);
            FileData data = (FileData) itemData;            

            seq = data.info.seq;
            isFile = data.info.isFile.Equals("Y");
            fileName = data.info.name;
            spaceSeq = data.spaceSeq;

            _imgIcon.sprite = isFile ? _sprFile : _sprFolder;
            _txtName.text = fileName;
            _txtSize.text = isFile ? data.info.fileSize.ToString("N0") +" KB" : string.Empty;
            _txtLastUpdate.text = data.info.dtm;
        }

        public void OnClick() => OnSelect();

        public void OnMenuClick() 
        {
            OnMenuClickAsync().Forget();
        }

        public async UniTaskVoid OnMenuClickAsync() 
        {
            string token = R.singleton.token;
            int workspaceSeq = R.singleton.workspaceSeq;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.GET_FULLPATH, workspaceSeq, spaceSeq, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.GET, string.Empty, token);
            if (string.IsNullOrEmpty(res.message)) 
            {
                this.fullPath = res.stringData;
                Debug.Log($"{TAG} | menu 출력을 준비하고 있습니다. fullpath : {res.stringData}");
            }
            else 
            {
                Debug.Log($"{TAG} | menu 출력 과정에서 오류 발생. {res.message}");
            }
        }

    #endregion  // GPM functions
    }
}