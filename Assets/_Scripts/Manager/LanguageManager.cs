using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LanguageManager
{
    private static List<SystemLanguage> availableLanguages = new List<SystemLanguage>()
    {
        ///
        /// ADD AVAILABLE LANGUAGES HERE
        /// 
        SystemLanguage.Spanish,
        SystemLanguage.English
    };

    private static int currentLanguageIndex;
    private static SystemLanguage currentLanguage
    {
        get
        {
            return availableLanguages[currentLanguageIndex];
        }
        set
        {
            currentLanguageIndex = availableLanguages.IndexOf(value);
        }
    }

    private static Dictionary<SystemLanguage, Dictionary<string, string>> _languageTable;
    private static Dictionary<SystemLanguage, Dictionary<string, string>> LanguageTable
    {
        get
        {
            if (_languageTable == null)
                LoadAllLanguagesTexts("LocalizationTexts");
            
            return _languageTable;
        }
    }

    private static void CheckUsersDeviceLanguage()
    {
        int languageSaved = PlayerPrefs.GetInt("LanguageSaved", 0);
        if (languageSaved != 0)
        {
            currentLanguageIndex = availableLanguages.IndexOf((SystemLanguage)languageSaved);
            return;
        }

        SystemLanguage sysLan = Application.systemLanguage;

        if (availableLanguages.Contains(sysLan) )
            currentLanguageIndex = availableLanguages.IndexOf( sysLan );
        else
            currentLanguageIndex = availableLanguages.IndexOf(SystemLanguage.English);

        PlayerPrefs.SetInt("CurrentLanguage", (int)availableLanguages[currentLanguageIndex]);
    }

    private static void LoadAllLanguagesTexts(string file)
    {
        CheckUsersDeviceLanguage();

        List<Dictionary<string, object>> rawData = CSVReader.Read(file);

        _languageTable = new Dictionary<SystemLanguage, Dictionary<string, string>>();
        for (int lineIndex = 0; lineIndex < rawData.Count; lineIndex++)
        {
            string key = "";
            foreach (KeyValuePair<string, object> entry in rawData[lineIndex])
            {
                if (entry.Key == "key")
                    key = entry.Value.ToString();

                SystemLanguage currentLanguage;
                if (Enum.TryParse<SystemLanguage>(entry.Key, out currentLanguage))
                {
                    if (_languageTable.ContainsKey(currentLanguage) == false)
                        _languageTable.Add(currentLanguage, new Dictionary<string, string>());

                    _languageTable[currentLanguage].Add(key, entry.Value.ToString());
                }
            }
        }
    }

    public static bool IsIndexValid(int index)
    {
        return index >= 0 && index < availableLanguages.Count;
    }

    public static string Localize(string key)
    {
        if (LanguageTable.ContainsKey(currentLanguage) == false)
            Debug.Log(currentLanguage);

        if ( LanguageTable.ContainsKey(currentLanguage) && LanguageTable[currentLanguage] != null && LanguageTable[currentLanguage].ContainsKey(key) )
            return LanguageTable[currentLanguage][key];
        else
            Debug.Log("Cannot find text " + key + " in language " + currentLanguage.ToString());

        return key;
    }

    public static List<SystemLanguage> GetAvailableLanguages()
    {
        return new List<SystemLanguage>(availableLanguages);
    }
}
