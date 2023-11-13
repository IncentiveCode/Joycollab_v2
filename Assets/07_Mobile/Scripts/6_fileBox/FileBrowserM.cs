/// <summary>
/// [mobile]
/// 파일 목록을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 19
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 19) : 최초 생성.
/// </summary>

using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

#if UNITY_ANDROID || UNITY_IOS
using NativeFilePickerNamespace;
#endif

namespace Joycollab.v2
{
    public class FileBrowserM : FixedView
    {
        private const string TAG = "FileBrowserM"; 

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputSearch;
        [SerializeField] private Button _btnClear;
        [SerializeField] private Button _btnSearch;

        [Header("link area")]
        [SerializeField] private Transform _transformLink;
        [SerializeField] private GameObject _goFileLink;

        [Header("button")]
        [SerializeField] private Button _btnBack;
        [SerializeField] private Button _btnNewFolder;
        [SerializeField] private Button _btnUpload;
        [SerializeField] private Button _btnPaste;

        [Header("content")]
        [SerializeField] private InfiniteScroll _scrollView;
        [SerializeField] private bool _isShare;

        // local variables
        private List<FileData> dataList;
        private int currentSpaceSeq;
        private string currentPath;

        private StringBuilder builder;
        private bool onSearch;


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
            viewID = ID.MobileScene_FileBox;


            // set 'search' inputfiled listener
            SetInputFieldListener(_inputSearch);
            _inputSearch.onValueChanged.AddListener((value) => {
                _btnClear.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
            _inputSearch.onSubmit.AddListener((value) => Debug.Log($"{TAG} | search, {value}"));
            _btnClear.onClick.AddListener(() => {
                _inputSearch.text = string.Empty;
                _inputSearch.Select();
            });


            // set button listener
            _btnBack.onClick.AddListener(() => ViewManager.singleton.Pop());
            _btnSearch.onClick.AddListener(() => Debug.Log($"{TAG} | search, {_inputSearch.text}"));
            _btnNewFolder.onClick.AddListener(() => Debug.Log($"{TAG} | create new folder."));
            _btnUpload.onClick.AddListener(() => Upload().Forget());
            _btnPaste.onClick.AddListener(() => Paste().Forget());


            // set scroll view listener
            _scrollView.AddSelectCallback((data) => {
                bool isFile = ((FileData)data).info.isFile.Equals("Y");

                if (isFile) 
                {
                    Debug.Log($"{TAG} | file select, name : {((FileData) data).info.name}");
                }
                else 
                {
                    string folderName = ((FileData) data).info.name;
                    string nextStep = string.Format("{0}/{1}", 
                        currentPath.Equals("/") ? string.Empty : currentPath, 
                        folderName
                    );
                    OpenPath(currentSpaceSeq, nextStep).Forget();
                }
            });


            // init local variables
            dataList = new List<FileData>();
            builder = new StringBuilder();
        }

        public override async UniTaskVoid Show() 
        {
            base.Show().Forget();
            await Refresh();
            base.Appearing();
        }

        public override async UniTaskVoid Show(string opt) 
        {
            base.Show().Forget();

            string[] temp = opt.Split('|');
            if (temp.Length < 2) 
            {
                Debug.Log($"{TAG} | Show(), 잘못된 opt 값. 확인 요망. {opt}");
                ViewManager.singleton.Pop();
                return;
            }

            int.TryParse(temp[0], out currentSpaceSeq);
            currentPath = temp[1];

            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView


    #region for list 

        private async UniTask<string> OpenPath(int spaceSeq, string path) 
        {
            currentSpaceSeq = spaceSeq;
            currentPath = path;

            // button 처리
            // _btnPaste.gameObject.SetActive(FileClipBoard.Instance.sourceSpaceSeq.Count > 0);
            _btnClear.gameObject.SetActive(string.IsNullOrEmpty(_inputSearch.text));

            string message = await GetList(spaceSeq, path);
            return message;
        }

        private void ClearLinks() 
        {
            Transform children = _transformLink.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Equals(_transformLink.name)) continue;
                Destroy(child.gameObject);
            }
        }

        private void MakeLinks(int spaceSeq, string path) 
        {
            // ROOT
            var link = Instantiate(_goFileLink, Vector3.zero, Quaternion.identity);
            link.GetComponent<FileLinkM>().Init("/");
            link.transform.SetParent(_transformLink, false);

            // 고정 폴더
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string rootName = string.Empty;

            switch (spaceSeq) 
            {
                case -1 : 
                    rootName = LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "폴더.개인", currentLocale);
                    break;

                case 0 :
                    rootName = LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "폴더.전사", currentLocale);
                    break;

                default :
                    rootName = LocalizationSettings.StringDatabase.GetLocalizedString("Menu", "폴더.부서", currentLocale);
                    break;
            }

            link = Instantiate(_goFileLink, Vector3.zero, Quaternion.identity);
            link.GetComponent<FileLinkM>().Init(rootName);
            link.GetComponent<Button>().onClick.AddListener(() => OpenPath(spaceSeq, "/").Forget());
            link.transform.SetParent(_transformLink, false);

            if (! string.IsNullOrEmpty(path)) 
            {
                string[] split = path.Split('/');
                builder.Clear();

                link = Instantiate(_goFileLink, Vector3.zero, Quaternion.identity);
                link.GetComponent<FileLinkM>().Init("/");
                link.transform.SetParent(_transformLink, false);

                int i = 1;
                foreach (string s in split) 
                {
                    if (string.IsNullOrEmpty(s)) continue;

                    link = Instantiate(_goFileLink, Vector3.zero, Quaternion.identity);
                    link.GetComponent<FileLinkM>().Init(s);
                    builder.Append("/");
                    builder.Append(s);
                    string temp = builder.ToString();
                    link.GetComponent<Button>().onClick.AddListener(() => OpenPath(spaceSeq, temp).Forget());
                    link.transform.SetParent(_transformLink, false);

                    if (i++ < split.Length - 1) 
                    {
                        link = Instantiate(_goFileLink, Vector3.zero, Quaternion.identity);
                        link.GetComponent<FileLinkM>().Init("/");
                        link.transform.SetParent(_transformLink, false);
                    }
                    
                }
            }
        }

        private async UniTask<string> GetList(int spaceSeq, string path) 
        {
            string token = R.singleton.token;
            int workpsaceSeq = R.singleton.workspaceSeq;

            if (path.Equals("/")) path = string.Empty;
            string url = string.Format(URL.FILE_LIST, workpsaceSeq, spaceSeq, path);

            PsResponse<ResFileList> res = await NetworkTask.RequestAsync<ResFileList>(url, eMethodType.GET, string.Empty, token);
            if (string.IsNullOrEmpty(res.message)) 
            {
                _scrollView.Clear();
                dataList.Clear();

                ClearLinks();
                MakeLinks(spaceSeq, path);

                FileData t;
                foreach (var item in res.data.list) 
                {
                    t = new FileData();
                    t.info = item;

                    dataList.Add(t);
                    _scrollView.InsertData(t);
                }
            }
            else 
            {
                _scrollView.Clear();
                dataList.Clear();

                PopupBuilder.singleton.OpenAlert(res.message);
            }

            return res.message;
        } 

        private async UniTaskVoid Upload() 
        {
            // 권한 체크
            bool privateFolder = (currentSpaceSeq == -1);
            bool auth = privateFolder ? true : R.singleton.CheckHasAuth(currentSpaceSeq, S.AUTH_UPLOAD_FILE);
            if (! auth) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "권한오류.파일 업로드", currentLocale)
                );

                await UniTask.Yield();
                return;
            }

            List<IMultipartFormSection> form = new List<IMultipartFormSection>();
            string section = "file";
            string[] fileTypes = new string[] { "*/*" };

        #if UNITY_ANDROID

            /**
            bool hasAuth = AndroidLib.singleton.CheckReadStorageAsync();
            if (hasAuth)
            {
                if (NativeFilePicker.IsFilePickerBusy()) 
                {
                    Debug.Log($"{TAG} | file picker is busy...");
                    return;
                }

                NativeFilePicker.Permission picker = NativeFilePicker.PickFile(
                    async (path) => { 
                        if (! string.IsNullOrEmpty(path)) 
                        {
                            // string[] split = path.Split(Path.DirectorySeparatorChar);
                            string[] split = path.Split('/');
                            Debug.Log($"{TAG} | file name : {split[split.Length-1]}"); 

                            byte[] fileData = File.ReadAllBytes(path);
                            form.Add(new MultipartFormFileSection(section, fileData, split[split.Length-1], fileTypes[0]));

                            string token = R.singleton.token;
                            int workspaceSeq = R.singleton.workspaceSeq;
                            string url = string.Format(URL.FILE_LIST, workspaceSeq, currentSpaceSeq, currentPath);
                            if (currentPath == "/") url = url.Replace("?folder=/", "");

                            PsResponse<string> res = await NetworkTask.PostMultipartAsync<string>(url, form, token);
                            if (string.IsNullOrEmpty(res.message)) 
                            {
                                OpenPath(currentSpaceSeq, currentPath).Forget();
                            }
                            else 
                            {
                                PopupBuilder.singleton.OpenAlert(res.message);
                            }
                        }
                    },
                    fileTypes
                );
            }
             */

            AndroidLib.singleton.CheckReadStorageAsync(async () => {
                if (NativeFilePicker.IsFilePickerBusy()) 
                {
                    Debug.Log($"{TAG} | file picker is busy...");
                    return;
                }

                NativeFilePicker.Permission permission = await NativeFilePicker.RequestPermissionAsync(false);
                Debug.Log($"{TAG} | Permission result : "+ permission);

                NativeFilePicker.Permission picker = NativeFilePicker.PickFile(
                    async (path) => { 
                        if (! string.IsNullOrEmpty(path)) 
                        {
                            // string[] split = path.Split(Path.DirectorySeparatorChar);
                            string[] split = path.Split('/');
                            Debug.Log($"{TAG} | file name : {split[split.Length-1]}"); 

                            byte[] fileData = File.ReadAllBytes(path);
                            form.Add(new MultipartFormFileSection(section, fileData, split[split.Length-1], fileTypes[0]));

                            string token = R.singleton.token;
                            int workspaceSeq = R.singleton.workspaceSeq;
                            string url = string.Format(URL.FILE_LIST, workspaceSeq, currentSpaceSeq, currentPath);
                            if (currentPath == "/") url = url.Replace("?folder=/", "");

                            PsResponse<string> res = await NetworkTask.PostMultipartAsync<string>(url, form, token);
                            if (string.IsNullOrEmpty(res.message)) 
                            {
                                OpenPath(currentSpaceSeq, currentPath).Forget();
                            }
                            else 
                            {
                                PopupBuilder.singleton.OpenAlert(res.message);
                            }
                        }
                    },
                    fileTypes
                );

            }).Forget();

        #elif UNITY_EDITOR

            string path = UnityEditor.EditorUtility.OpenFilePanel("pick", "", "");
            if (! string.IsNullOrEmpty(path)) 
            {
                // string[] split = path.Split(Path.DirectorySeparatorChar);
                string[] split = path.Split('/');
                Debug.Log($"{TAG} | file name : {split[split.Length-1]}"); 

                byte[] fileData = File.ReadAllBytes(path);
                form.Add(new MultipartFormFileSection(section, fileData, split[split.Length-1], fileTypes[0]));

                string token = R.singleton.token;
                int workspaceSeq = R.singleton.workspaceSeq;
                string url = string.Format(URL.FILE_LIST, workspaceSeq, currentSpaceSeq, currentPath);
                if (currentPath == "/") url = url.Replace("?folder=/", "");

                PsResponse<string> res = await NetworkTask.PostMultipartAsync<string>(url, form, token);
                if (string.IsNullOrEmpty(res.message)) 
                {
                    OpenPath(currentSpaceSeq, currentPath).Forget();
                }
                else 
                {
                    PopupBuilder.singleton.OpenAlert(res.message);
                }                
            }
        
        #endif
        }

        // 실험용 함수
        private async UniTaskVoid Paste()
        {
            // 권한 체크
            bool privateFolder = (currentSpaceSeq == -1);
            bool auth = privateFolder ? true : R.singleton.CheckHasAuth(currentSpaceSeq, S.AUTH_UPLOAD_FILE);
            if (! auth) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                PopupBuilder.singleton.OpenAlert(
                    LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "권한오류.파일 업로드", currentLocale)
                );

                await UniTask.Yield();
                return;
            }

            List<IMultipartFormSection> form = new List<IMultipartFormSection>();
            string section = "file";
            string[] fileTypes = new string[] { "*/*" };

            AndroidLib.singleton.CheckWriteStorageAsync(async () => {
                if (NativeFilePicker.IsFilePickerBusy()) 
                {
                    Debug.Log($"{TAG} | file picker is busy...");
                    return;
                }

                NativeFilePicker.Permission permission = await NativeFilePicker.RequestPermissionAsync(false);
                Debug.Log($"{TAG} | Permission result : "+ permission);

                NativeFilePicker.Permission picker = NativeFilePicker.PickFile(
                    async (path) => { 
                        if (! string.IsNullOrEmpty(path)) 
                        {
                            // string[] split = path.Split(Path.DirectorySeparatorChar);
                            string[] split = path.Split('/');
                            Debug.Log($"{TAG} | file name : {split[split.Length-1]}"); 

                            byte[] fileData = File.ReadAllBytes(path);
                            form.Add(new MultipartFormFileSection(section, fileData, split[split.Length-1], fileTypes[0]));

                            string token = R.singleton.token;
                            int workspaceSeq = R.singleton.workspaceSeq;
                            string url = string.Format(URL.FILE_LIST, workspaceSeq, currentSpaceSeq, currentPath);
                            if (currentPath == "/") url = url.Replace("?folder=/", "");

                            PsResponse<string> res = await NetworkTask.PostMultipartAsync<string>(url, form, token);
                            if (string.IsNullOrEmpty(res.message)) 
                            {
                                OpenPath(currentSpaceSeq, currentPath).Forget();
                            }
                            else 
                            {
                                PopupBuilder.singleton.OpenAlert(res.message);
                            }
                        }
                    },
                    fileTypes
                );
            }).Forget();
        }

    #endregion  // for list 


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // get list
            Debug.Log($"{TAG} | Refresh(), space seq : {currentSpaceSeq}, path : {currentPath}");
            OpenPath(currentSpaceSeq, currentPath).Forget();

            await UniTask.Yield();
            return 0;
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
            if (onSearch) 
            {
                // TODO. search panel clear.

                onSearch = false;
                return;
            }

            if (currentPath.Equals("/")) 
            {
                ViewManager.singleton.Pop();
            }
            else 
            {
                string[] paths = currentPath.Split('/');
                builder.Clear();

                for (int i = 0; i < paths.Length - 1; i++) 
                {
                    if (string.IsNullOrEmpty(paths[i])) continue;

                    builder.Append("/");
                    builder.Append(paths[i]);
                }

                string prevPath = builder.ToString();
                Debug.Log($"{TAG} | prev path : {prevPath}");
                if (string.IsNullOrEmpty(prevPath)) 
                    currentPath = "/";
                else                            
                    currentPath = prevPath;
                Refresh().Forget();
            }
        }

    #endregion  // event handling
    }
}