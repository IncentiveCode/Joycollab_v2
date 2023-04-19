using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2 
{
    public class LocaleManager : MonoBehaviour
    {
        public string Region { get; private set; }
        private bool isChanging;

        public void ChangeLocale(int locale) 
        {
            if (isChanging) return;

            StartCoroutine(Change(locale));
        }

        private IEnumerator Change(int locale) 
        {
            isChanging = true;

            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[locale];

            isChanging = false;
        }
    }
}