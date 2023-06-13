/// <summary>
/// Unity Localization - dropdown 적용 
/// @author         : UVH, HJ Lee
/// @reference      : https://sonsazang.tistory.com/18
/// @last update    : 2023. 06. 13
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 13) : 검색 후 TMP 형태로 변경해서 적용
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

namespace Joycollab.v2
{
    [RequireComponent(typeof(TMP_Dropdown))]
    [AddComponentMenu("Localization/Localize Dropdown")]
    public class LocalizeDropdown : MonoBehaviour
    {
        [Serializable]
        public class LocalizedDropdownOption
        {
            public LocalizedString text;
        }

        public List<LocalizedDropdownOption> options;
        public int selectedOptionIndex = 0;
        private Locale currentLocale = null;
        private TMP_Dropdown Dropdown => GetComponent<TMP_Dropdown>();


    #region Unity functions

        private void Start()
        {
            getLocale();
            UpdateDropdown(currentLocale);
            LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;
        }

        private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;
        private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;
        void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;

    #endregion  // Unity functions


        private void getLocale()
        {
            var locale = LocalizationSettings.SelectedLocale;
            if (currentLocale != null && locale != currentLocale)
            {
                currentLocale = locale;
            }
        }

        private void UpdateDropdown(Locale locale)
        {
            selectedOptionIndex = Dropdown.value;
            Dropdown.ClearOptions();

            for (int i = 0; i < options.Count; i++)
            {
                String localizedText = options[i].text.GetLocalizedString();
                Dropdown.options.Add(new TMP_Dropdown.OptionData(localizedText));
            }

            Dropdown.value = selectedOptionIndex;
            Dropdown.RefreshShownValue();
        }
    }
}