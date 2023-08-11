/// <summary>
/// PlayerPrefs Key list 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 04. 19
/// @version        : 0.3
/// @update
///     v0.1 (2023. 03. 21) : Joycollab 에서 사용하던 클래스 정리 및 통합 1
///     v0.2 (2023. 03. 22) : Joycollab 에서 사용하던 클래스 정리 및 통합 2
///     v0.3 (2023. 04. 19) : Joycollab 에서 사용하던 클래스 정리 및 통합 3
/// </summary>

namespace Joycollab.v2
{
	public class Key
    {
	#region for token

		public const string TOKEN = "_t";
		public const string ACCESS_TOKEN = "access_token";
		public const string TOKEN_TYPE = "_tt";

	#endregion	// for token


	#region for param check

		public const string WORKSPACE_SEQ = "workspaceSeq";
		public const string EMAIL = "email";
		public const string CKEY = "ckey";
		public const string USER_NAME = "userName";
    	public const string GUEST_LOGIN = "glink";
		public const string DOMAIN = "domain";
		public const string JOIN = "join";

	#endregion	// for param check


    #region for Login

		public const string WORLD = "_world";
		public const string INVITED = "_invited";
		public const string FREETRIAL = "_freetrial";
		public const string SYSTEM_UPDATE_FLAG = "_suf";

		public const string TOGGLE_ID_SAVED = "_tis";
	 	public const string SAVED_LOGIN_ID = "_sli";
		public const string TOGGLE_SUB_ID_SAVED = "_tsis";
		public const string SAVED_SUB_LOGIN_ID = "_ssli";
		public const string TOGGLE_WORLD_ID_SAVED = "_twis";
		public const string SAVED_WORLD_LOGIN_ID = "_swli";
		public const string TOGGLE_GO_TO_CENTER = "_tgtc";

		public const string WORKSPACE_DOMAIN = "_wd";
		public const string WORKSPACE_NAME = "_wn";
		public const string WORKSPACE_LOGO = "_wl";
		public const string WORKSPACE_END_DATE = "_wed";

    	public const string MEMBER_SEQ = "_ms";

		public const string GUEST_ID = "_gi";
		public const string GUEST_PASSWORD = "_gp";

    #endregion  // for Login


	#region for Mobile

		public const string MOBILE_FIRST_PAGE = "_mfp";
		public const string MOBILE_GESTURE_PROPERTIES = "_mgr";

	#endregion	// for Mobile
	}
}