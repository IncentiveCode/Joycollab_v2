/// <summary>
/// To-Do 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 06. 12
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 12) : 최초 생성 및 작업 중.
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class ToDoModule : MonoBehaviour
    {
        private const string TAG = "ToDoModule";

        [SerializeField] private Transform _transformList;
        [SerializeField] private GameObject _goItem;


    #region public functions

        public async UniTask<string> GetList(ReqToDoList req, bool refresh=true)
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = req.url;
            PsResponse<ResToDoList> res = await NetworkTask.RequestAsync<ResToDoList>(url, eMethodType.GET, string.Empty, token);

            return res.message;
        }

    #endregion  // public functions
    }
}