/// <summary>
/// 모임방 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 09. 22
/// @version        : 0.2
/// @update
///     v0.1 (2023. 09. 14) : 최초 생성
/// 	v0.2 (2023. 09. 22) : 카테고리 목록 확인, 모임방 생성 기능 추가.
/// </summary>

using UnityEngine;
using Gpm.Ui;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
	public class GatheringModule : MonoBehaviour
	{
		private const string TAG = "GatheringModule";


	#region for List 

		public async UniTask<string> GetRoomList(RequestForClas req, InfiniteScroll view) 
		{
			string token = R.singleton.token;
        	PsResponse<ClasList> res = await NetworkTask.RequestAsync<ClasList>(req.url, eMethodType.GET, string.Empty, token);

        	if (string.IsNullOrEmpty(res.message))
			{
				if (req.refresh) 
				{
					view.Clear();
				}

				ClasData t;
				foreach (var item in res.data.content) 
				{
					t = new ClasData(item);
					view.InsertData(t);
				}

				if (res.data.hasNext) 
				{
					t = new ClasData();
					view.InsertData(t);
				}
			}
			else 
			{
				view.Clear();
			}

			return res.message;
		}

        public async UniTask<PsResponse<TpsList>> GetCategories() 
        {
            string url = string.Format(URL.GET_CODE, "모임방 카테고리");
            PsResponse<TpsList> res = await NetworkTask.RequestAsync<TpsList>(url, eMethodType.GET, string.Empty, R.singleton.token);

            return res; 
        }

		public async UniTask<string> GetCategoryList(InfiniteScroll view) 
		{
            string url = string.Format(URL.GET_CODE, "모임방 카테고리");
            PsResponse<TpsList> res = await NetworkTask.RequestAsync<TpsList>(url, eMethodType.GET, string.Empty, R.singleton.token);

			view.Clear();
			if (string.IsNullOrEmpty(res.message)) 
			{
				RoomCategoryData t;
				foreach (var item in res.data.list) 
				{
					t = new RoomCategoryData(item);
					view.InsertData(t);
				}
			}

			return res.message;
		}

	#endregion	// for List
	}
}