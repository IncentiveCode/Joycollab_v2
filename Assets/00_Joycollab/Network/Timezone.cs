/// <summary>
/// Network 통신 - Timezone 관련 응답 
/// @author         : HJ Lee
/// @last update    : 2023. 10. 11
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 11) : tp 에서 사용하던 클래스 정리 및 통합 
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    [Serializable] 
    public class TimezoneList 
    {
        public List<TimezoneItem> list; 

        public TimezoneList() 
        {
            list = new List<TimezoneItem>();
            list.Clear();
        }
    }

    [Serializable]
    public class TimezoneItem 
    {
        public string id;
        public string offset;
        public int utcOffset;

        public TimezoneItem(string id, string offset, int utcOffset) 
        {
            this.id = id;
            this.offset = offset;
            this.utcOffset = utcOffset;
        }

        public string TimezoneName() 
        {
            string timezoneId = LocalizationSettings.StringDatabase.GetLocalizedString("Timezone", this.id, R.singleton.CurrentLocale);
            string content = string.Format("{0} ({1})", timezoneId, this.offset);
            return content;
        }
    }
}