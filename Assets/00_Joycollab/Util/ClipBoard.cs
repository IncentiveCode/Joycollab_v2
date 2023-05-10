/// <summary>
/// Ctrl-C + Ctrl-V 를 사용하기 위한 클립보드 클래스
/// @author         : HJ Lee
/// @last update    : 2021. 07. 02
/// @version        : 0.1
/// @update
///     v0.1 (2021. 07. 02) : 최초 생성
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class ClipBoard : Singleton<ClipBoard>
    {
        private TextEditor te;

        public string contents 
        {
            get
            {
                if (te == null) te = new TextEditor();
                te.Paste();
                return te.text;
            }

        set
            {
                if (te == null) te = new TextEditor();
                te.text = value;
                te.OnFocus();
                te.Copy();
            }
        }
    }
}