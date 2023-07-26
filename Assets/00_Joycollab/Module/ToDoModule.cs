/// <summary>
/// To-Do 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 07. 21
/// @version        : 0.2
/// @update
///     v0.1 (2023. 06. 12) : 최초 생성 
///     v0.2 (2023. 07. 21) : GetList() 수정.
///     v0.3 (2023. 07. 25) : GetList() -> Get() 으로 이름 변경. Search() 추가.
/// </summary>

using UnityEngine;
using Gpm.Ui;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class ToDoModule : MonoBehaviour
    {
        private const string TAG = "ToDoModule";


    #region public functions

        public async UniTask<string> Get(InfiniteScroll view, ReqToDoList req, bool refresh=true) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = req.url;
            PsResponse<ResToDoList> res = await NetworkTask.RequestAsync<ResToDoList>(url, eMethodType.GET, string.Empty, token);

            // 에러 리턴.
            if (! string.IsNullOrEmpty(res.message)) 
            {
                view.Clear();
                Tmp.singleton.ClearToDoList();

                return res.message;
            }

            // 리스트 정리.
            if (refresh) 
            {
                view.Clear();
                Tmp.singleton.ClearToDoList();
            }

            ToDoData t;
            foreach (var item in res.data.content) 
            {
                t = new ToDoData();
                t.info = item;
                t.loadMore = false;

                view.InsertData(t);
                Tmp.singleton.AddToDoInfo(item.seq, t);
            }

            if (res.data.hasNext) 
            {
                t = new ToDoData();
                t.loadMore = true;
                view.InsertData(t);
            }

            return string.Empty;
        }

        public async UniTask<string> Search(InfiniteScroll view, ReqToDoList req, bool refresh=true) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = req.url;
            PsResponse<ResToDoList> res = await NetworkTask.RequestAsync<ResToDoList>(url, eMethodType.GET, string.Empty, token);

            // 에러 리턴.
            if (! string.IsNullOrEmpty(res.message)) 
            {
                view.Clear();
                Tmp.singleton.ClearToDoSearchList();

                return res.message;
            }

            // 리스트 정리.
            if (refresh)
            {
                view.Clear();
                Tmp.singleton.ClearToDoSearchList();
            }

            ToDoData t;
            foreach (var item in res.data.content) 
            {
                t = new ToDoData();
                t.info = item;
                t.loadMore = false;

                view.InsertData(t);
                Tmp.singleton.AddSearchToDo(item.seq, t);
            }

            if (res.data.hasNext)
            {
                t = new ToDoData();
                t.loadMore = true;
                view.InsertData(t);
            }

            return string.Empty;
        }

        public async UniTask<PsResponse<string>> CheckItem(int seq) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.CONTROL_TODO, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, string.Empty, token);

            return res;
        }

        public async UniTask<PsResponse<string>> SaveToDo(string body, int shareOpt, int remindOpt)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.REGIST_TODO, memberSeq, shareOpt, remindOpt);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, token);

            return res;
        }

        public async UniTask<PsResponse<string>> UpdateToDo(int seq, string body, int shareOpt, int remindOpt)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.MODIFY_TODO, memberSeq, seq, shareOpt, remindOpt);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, body, token);

            return res;
        }

        public async UniTask<PsResponse<string>> DeleteToDo(int seq) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.CONTROL_TODO, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.DELETE, string.Empty, token);

            return res;
        }

    #endregion  // public functions
    }
}