/// <summary>
/// 자주 사용하는 color 값 정리 문서
/// @author         : HJ Lee
/// @last update    : 2023. 04. 14
/// @version        : 0.1
/// @update
///     v0.1 (2023. 04. 14) : 최초 생성. 상태 색상, 버튼 색상 정리  
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
	public class C 
	{
	#region state color

		/// <summary>
		/// avatar state - 온라인 - #00E532 
		/// </summary>
        public static readonly Color ONLINE = new Color(0f, 0.8980392f, 0.1960784f, 1f);

		/// <summary>
		/// avatar state - 오프라인 - #858383
		/// </summary>
        public static readonly Color OFFLINE = new Color(0.5215687f, 0.5137255f, 0.5137255f, 1f); 

		/// <summary>
		/// avatar state - 회의중 - #FF856A 
		/// </summary>
        public static readonly Color MEETING = new Color(1f, 0.5215687f, 0.4156863f, 1f);

		/// <summary>
		/// avatar state - 통화중 - #FFB843
		/// </summary>
        public static readonly Color LINE_BUSY = new Color(1f, 0.7215686f, 0.2627451f, 1f);

		/// <summary>
		/// avatar state - 바쁨 - #E81523
		/// </summary>
        public static readonly Color BUSY = new Color(0.9098039f, 0.08235294f, 0.1372549f, 1f);

		/// <summary>
		/// avatar state - 외근중 - #8AC7FF
		/// </summary>
        public static readonly Color OUT_ON_BUSINESS = new Color(0.5411765f, 0.7803922f, 1f, 1f);

		/// <summary>
		/// avatar state - 자리비움 - #0E9EEF
		/// </summary>
        public static readonly Color NOT_HERE = new Color(0.05490196f, 0.6196079f, 0.9372549f, 1f);

		/// <summary>
		/// avatar state - 방해금지 - #DF0000
		/// </summary>
        public static readonly Color DO_NOT_DISTURB = new Color(0.8745098f, 0f, 0f, 1f);

		/// <summary>
		/// avatar state - 휴가중 - #49A6BA
		/// </summary>
        public static readonly Color VACATION = new Color(0.2862745f, 0.6509804f, 0.7294118f, 1f);

		/// <summary>
		/// avatar state - 부재중 - #7987FF
		/// </summary>
        public static readonly Color NOT_AVAILABLE = new Color(0.4745098f, 0.5294118f, 1f, 1f);

	#endregion	// state color


	#region button color

		/// <summary>
		/// workspace - normal - #FD7D00 
		/// </summary>
        public static readonly Color NORMAL = new Color(0.9921569f, 0.4901961f, 0f, 1f);

		/// <summary>
		/// workspace - normal - mouse over & click - #E85A0B
		/// </summary>
        public static readonly Color NORMAL_ON = new Color(0.9098039f, 0.3529412f, 0.04313726f, 1f);

		/// <summary>
		/// workspace - warning - #FFFFFF 
		/// </summary>
        public static readonly Color WARNING = new Color(1f, 1f, 1f, 1f);

		/// <summary>
		/// workspace - warning - mouse over & click - #F5F5F5
		/// </summary>
        public static readonly Color WARNING_ON = new Color(0.9607843f, 0.9607843f, 0.9607843f, 1f);

		/// <summary>
		/// workspace - warning - border - #D9D9D9
		/// </summary>
        public static readonly Color WARNING_BORDER = new Color(0.8509804f, 0.8509804f, 0.8509804f, 1f);

		/// <summary>
		/// world - normal - #6E5184
		/// </summary>
        public static readonly Color WORLD_NORMAL = new Color(0.4313726f, 0.3176471f, 0.5176471f, 1f);

		/// <summary>
		/// world - normal - mouse over & click - #64477A
		/// </summary>
        public static readonly Color WORLD_NORMAL_ON = new Color(0.3921569f, 0.2784314f, 0.4784314f, 1f);

		/// <summary>
		/// world - warning - #D5CBDC
		/// </summary>
        public static readonly Color WORLD_WARNING = new Color(0.8352941f, 0.7960784f, 0.8627451f, 1f);

		/// <summary>
		/// world - warning - mouse over & click - #CBC1D2
		/// </summary>
        public static readonly Color WORLD_WARNING_ON = new Color(0.7960784f, 0.7568628f, 0.8235294f, 1f);

	#endregion	// button color
	}
}