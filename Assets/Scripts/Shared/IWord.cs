using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IWord
{
    public string id;
    public string type;
    public string[] words;
}

[Serializable]
public class IWordArray
{
    public List<IWord> words;

    public static IWordArray ParseJson(string jsonString)
    {
        return JsonUtility.FromJson<IWordArray>(jsonString);
    }
}