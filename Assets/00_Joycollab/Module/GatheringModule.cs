/// <summary>
/// 모임방 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 09. 14
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 14) : 최초 생성
/// </summary>

using UnityEngine;
using Gpm.Ui;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
	public class GatheringModule : MonoBehaviour
	{
		private const string TAG = "GatheringModule";


	#region public functions

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

	#endregion	// public functions
	}
}