using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum GameType
{
    Opposite = 5000,
    Synonym = 10000,
    SingleWord = 40000
}

[Serializable]
public class GameState
{
    public string m_strTitle;
    public int m_iLevel;
    public int m_iPrice;
    public bool m_bBlocked;
    public GameType m_gameType;
    [System.NonSerialized] public List<List<string>> m_lWords;

    public GameState(string title, GameType gameType)
    {
        m_strTitle = title;
        m_iLevel = 1;
        m_gameType = gameType;
        m_bBlocked = true;
        m_iPrice = (int)gameType;
        m_lWords = new List<List<string>>();
    }

    public override string ToString()
    {
        switch (m_gameType)
        {
            case GameType.Opposite:
                return "Opposite";
            case GameType.Synonym:
                return "Synonym";
            case GameType.SingleWord:
                return "SingleWord";
            default:
                return "Unknown Game";
        }

    }
}
