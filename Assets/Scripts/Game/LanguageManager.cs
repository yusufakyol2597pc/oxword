using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(Instance);
        else
            Instance = this;
    }

    [SerializeField]
    private LocalizedStringTable _localizedStringTable;

    public string Translate(string key)
    {
        StringTable currentStringTable = _localizedStringTable.GetTable();
        if (currentStringTable != null)
        {
            var entry = currentStringTable[key];
            return entry.GetLocalizedString();
        }
        return "Table not found!";
    }

    public string Translate(string key, Dictionary<string, string> dict)
    {
        StringTable currentStringTable = _localizedStringTable.GetTable();
        if (currentStringTable != null)
        {
            var entry = currentStringTable[key];
            return entry.GetLocalizedString(dict);
        }
        return "Table not found!";
    }
}
