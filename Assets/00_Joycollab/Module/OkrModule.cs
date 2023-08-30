/// <summary>
/// OKR 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 07. 26
/// @version        : 0.2
/// @update
///     v0.1 (2023. 07. 04) : 최초 생성 및 작업 중.
///     v0.2 (2023. 07. 26) : GetList() 수정. GetList() -> Get() 으로 이름 변경. Search() 추가.
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using Gpm.Ui;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class OkrModule : MonoBehaviour
    {
        private const string TAG = "OkrModule";


    #region public functions

        public async UniTask<string> Get(InfiniteScroll view, ReqOkrList req, bool refresh=true)
        {
            string token = R.singleton.token;

            string url = req.url;
            PsResponse<ResOkrList> res = await NetworkTask.RequestAsync<ResOkrList>(url, eMethodType.GET, string.Empty, token);

            // 에러 리턴.
            if (! string.IsNullOrEmpty(res.message)) 
            {
                view.Clear();
                Tmp.singleton.ClearOkrList();

                return res.message;
            }

            // 리스트 정리.
            List<int> listKeyResult = new List<int>();
            listKeyResult.Clear();

            if (refresh) 
            {
                view.Clear();
                Tmp.singleton.ClearOkrList();
            }

            OkrData t;

            // 공유 OKR
            if (req.share) 
            {
                // - key result 정리
                foreach (var item in res.data.content) 
                {
                    if (! string.IsNullOrEmpty(item.topOkr.title)) 
                        listKeyResult.Add(item.seq);
                }

                // - objective 출력
                foreach (var item in res.data.content) 
                {
                    if (! string.IsNullOrEmpty(item.topOkr.title)) continue;

                    t = new OkrData(item);
                    view.InsertData(t);
                    Tmp.singleton.AddOkrInfo(item.seq, t);

                    // -- 하위 key result 출력
                    foreach (var subItem in res.data.content) 
                    {
                        if (item.seq == subItem.topOkr.seq && listKeyResult.Contains(subItem.seq)) 
                        {
                            t = new OkrData(subItem, true);
                            view.InsertData(t);
                            Tmp.singleton.AddOkrInfo(subItem.seq, t);

                            listKeyResult.Remove(subItem.seq);
                        }
                    }
                }
            }
            // 개인 OKR
            else 
            {
                // - key result 정리
                foreach (var item in res.data.content) 
                {
                    if (item.shereType == 0) 
                        listKeyResult.Add(item.seq);
                }

                // objective 출력
                foreach (var item in res.data.content) 
                {
                    if (item.shereType == 0) continue;

                    t = new OkrData(item);
                    view.InsertData(t);
                    Tmp.singleton.AddOkrInfo(item.seq, t);

                    // key result 출력
                    foreach (var subItem in item.subOkr) 
                    {
                        t = new OkrData(subItem, item.shereType, item.title);
                        view.InsertData(t);
                        Tmp.singleton.AddOkrInfo(subItem.seq, t);

                        listKeyResult.Remove(subItem.seq);
                    }
                }
            }

            // 남은 key result 출력
            if (listKeyResult.Count > 0) 
            {
                foreach (var subItem in res.data.content) 
                {
                    if (listKeyResult.Contains(subItem.seq))
                    {
                        t = new OkrData(subItem, true);
                        view.InsertData(t);
                        Tmp.singleton.AddOkrInfo(subItem.seq, t);

                        listKeyResult.Remove(subItem.seq);
                    } 
                }     
            }

            if (res.data.hasNext) 
            {
                t = new OkrData();
                t.loadMore = true;
                view.InsertData(t);
            }

            return string.Empty;
        }

        public async UniTask<string> Search(InfiniteScroll view, ReqOkrList req, bool refresh=true) 
        {
            string token = R.singleton.token;

            string url = req.url;
            PsResponse<ResOkrList> res = await NetworkTask.RequestAsync<ResOkrList>(url, eMethodType.GET, string.Empty, token);

            // 에러 리턴.
            if (! string.IsNullOrEmpty(res.message)) 
            {
                view.Clear();
                Tmp.singleton.ClearOkrSearchList();

                return res.message;
            }

            // 리스트 정리.
            List<int> listKeyResult = new List<int>();
            listKeyResult.Clear();

            if (refresh) 
            {
                view.Clear();
                Tmp.singleton.ClearOkrSearchList();
            }

            OkrData t;

            // 공유 OKR
            if (req.share) 
            {
                // - key result 정리
                foreach (var item in res.data.content) 
                {
                    if (! string.IsNullOrEmpty(item.topOkr.title)) 
                        listKeyResult.Add(item.seq);
                }

                // - objective 출력
                foreach (var item in res.data.content) 
                {
                    if (! string.IsNullOrEmpty(item.topOkr.title)) continue;

                    t = new OkrData(item);
                    view.InsertData(t);
                    Tmp.singleton.AddSearchOkr(item.seq, t);

                    // -- 하위 key result 출력
                    foreach (var subItem in res.data.content) 
                    {
                        if (item.seq == subItem.topOkr.seq && listKeyResult.Contains(subItem.seq)) 
                        {
                            t = new OkrData(subItem, true);
                            view.InsertData(t);
                            Tmp.singleton.AddSearchOkr(subItem.seq, t);

                            listKeyResult.Remove(subItem.seq);
                        }
                    }
                }
            }
            // 개인 OKR
            else 
            {
                // - key result 정리
                foreach (var item in res.data.content) 
                {
                    if (item.shereType == 0) 
                        listKeyResult.Add(item.seq);
                }

                // objective 출력
                foreach (var item in res.data.content) 
                {
                    if (item.shereType == 0) continue;

                    t = new OkrData(item);
                    view.InsertData(t);
                    Tmp.singleton.AddSearchOkr(item.seq, t);

                    // key result 출력
                    foreach (var subItem in item.subOkr) 
                    {
                        t = new OkrData(subItem, item.shereType, item.title);
                        view.InsertData(t);
                        Tmp.singleton.AddSearchOkr(subItem.seq, t);

                        listKeyResult.Remove(subItem.seq);
                    }
                }
            }

            // 남은 key result 출력
            if (listKeyResult.Count > 0) 
            {
                foreach (var subItem in res.data.content) 
                {
                    if (listKeyResult.Contains(subItem.seq))
                    {
                        t = new OkrData(subItem, true);
                        view.InsertData(t);
                        Tmp.singleton.AddSearchOkr(subItem.seq, t);

                        listKeyResult.Remove(subItem.seq);
                    } 
                }     
            }

            if (res.data.hasNext) 
            {
                t = new OkrData();
                t.loadMore = true;
                view.InsertData(t);
            }

            return string.Empty;
        }

        public async UniTask<PsResponse<TopOkrList>> GetObjectives(int share)
        { 
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.OBJECTIVE_LIST, memberSeq, share);
            PsResponse<TopOkrList> res = await NetworkTask.RequestAsync<TopOkrList>(url, eMethodType.GET, string.Empty, token);

            return res;
	    }

        public async UniTask<PsResponse<string>> SaveObjective(string body, int shareOpt)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.REGIST_OBJECTIVE, memberSeq, shareOpt);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, token);

            return res;
        }

        public async UniTask<PsResponse<string>> SaveKeyResult(string body, int objectiveSeq)
        { 
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.REGIST_KEY_RESULT, memberSeq, objectiveSeq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, token);

            return res;
	    }

        public async UniTask<PsResponse<string>> UpdateOkr(int seq, string body)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.CONTROL_OKR, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.PUT, body, token);

            return res;
        }

        public async UniTask<PsResponse<string>> DeleteOkr(int seq) 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.CONTROL_OKR, memberSeq, seq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.DELETE, string.Empty, token);

            return res;
        }

    #endregion  // public functions
    }
}