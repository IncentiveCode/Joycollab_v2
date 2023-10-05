/// <summary>
/// 월드 API 테스트
/// @author         : HJ Lee
/// @last update    : 2023. 10. 05 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 08. 31) : 최초 생성
///     v0.2 (2023. 10. 05) : assembly 관련 이슈로 인해 잠시 주석처리.
/// </summary>

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class WorldApiTest
    {
        private const string TAG = "WorldApiTest";

        private int workspaceSeq;
        private string token;

        static int[] ClasSeqs = new int[] { 260, 261, 262 };


        /**
        [UnityTest]
        public IEnumerator W0_Get_UserToken() => UniTask.ToCoroutine(async () => {
            string temp = string.Empty; 

            string basicToken = NetworkTask.BASIC_TOKEN;
            string id = "hjlee@pitchsolution.co.kr";
            string pw = "Qwer!234";

            WWWForm form = new WWWForm();
            form.AddField(NetworkTask.GRANT_TYPE, NetworkTask.GRANT_TYPE_PW);
            form.AddField(NetworkTask.PASSWORD, pw);
            form.AddField(NetworkTask.REFRESH_TOKEN, string.Empty);
            form.AddField(NetworkTask.SCOPE, NetworkTask.SCOPE_ADM);
            form.AddField(NetworkTask.USERNAME, id);

            PsResponse<ResToken> res = await NetworkTask.PostAsync<ResToken>(URL.REQUEST_TOKEN, form, string.Empty, basicToken); 
            if (string.IsNullOrEmpty(res.message))
            {
                temp = string.Format("{0} {1}", res.data.token_type, res.data.access_token);
            }

            Debug.Log($"{TAG} | W0_Get_UserToken(), result : {temp}");
            this.token = temp;

            Assert.AreNotEqual(temp, string.Empty);
        });


        [UnityTest]
        public IEnumerator W0_Get_World_Sequence() => UniTask.ToCoroutine(async () => {
            int temp = -1;

            string url = string.Format(URL.CHECK_DOMAIN, S.WORLD);
            PsResponse<SimpleWorkspace> res = await NetworkTask.RequestAsync<SimpleWorkspace>(url, eMethodType.GET);
            if (string.IsNullOrEmpty(res.message))
            {
                temp = res.data.seq;
            }
            Debug.Log($"{TAG} | W0_Get_World_Sequence(), result : {temp}");
            workspaceSeq = temp;

            Assert.AreNotEqual(temp, -1);
        });

        [UnityTest]
        public IEnumerator W1_Get_World_Option() => UniTask.ToCoroutine(async () => {
            string url = string.Format(URL.WORLD_INFO, workspaceSeq);

            await UniTask.Yield();
        });

        [UnityTest]
        public IEnumerator W2_Get_Clas_List() => UniTask.ToCoroutine(async () => {
            int temp = -1;

            string url = string.Format(URL.CLAS_LIST, 1, 20);
            PsResponse<ClasList> res = await NetworkTask.RequestAsync<ClasList>(url, eMethodType.GET, string.Empty, token);
            if (string.IsNullOrEmpty(res.message))
            {
                temp = res.data.totalElements;

                Debug.Log($"{TAG} | W2_Get_Clas_List(), count : {res.data.totalElements}");
                foreach (var info in res.data.content) 
                {
                    Debug.Log($">> seq : {info.seq}, name : {info.nm}, bigo : {info.clas.bigo}");
                }
            }

            Assert.AreNotEqual(temp, -1);
        });

        [UnityTest]
        public IEnumerator W3_Get_Clas_Info([ValueSource("ClasSeqs")] int seq) => UniTask.ToCoroutine(async () => {
            string url = string.Format(URL.CLAS_INFO, seq);
            PsResponse<ClasDetail> res = await NetworkTask.RequestAsync<ClasDetail>(url, eMethodType.GET, string.Empty, token);
            if (string.IsNullOrEmpty(res.message))
            {
                Debug.Log($"{TAG} | W3_Get_Clas_Info(), seq : {res.data.seq}, bigo : {res.data.bigo}");
            }

            Assert.AreEqual(res.message, string.Empty);
        });
         */
    }
}