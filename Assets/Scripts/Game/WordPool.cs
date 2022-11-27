using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using Random = System.Random;

class WordGroup
{
    int m_letterCount;
    List<Tuple<string, string>> m_words;

    WordGroup(int letterCount, List<Tuple<string, string>> words)
    {
        m_letterCount = letterCount;
        m_words = words;
    }
}

public class WordPool : MonoBehaviour
{
    public static WordPool Instance;

    [SerializeField] TextAsset m_taSynomymWords;
    [SerializeField] TextAsset m_taOppositeWords;
    [SerializeField] TextAsset m_taSingleWords;

    IDictionary<int, List<string>> m_synonymAllWords = new Dictionary<int, List<string>>();
    IDictionary<int, List<string>> m_synonymDoneWords = new Dictionary<int, List<string>>();

    IDictionary<int, List<string>> m_oppositeAllWords = new Dictionary<int, List<string>>();
    IDictionary<int, List<string>> m_oppositeDoneWords = new Dictionary<int, List<string>>();

    IDictionary<int, List<string>> m_singleAllWords = new Dictionary<int, List<string>>();
    IDictionary<int, List<string>> m_singleDoneWords = new Dictionary<int, List<string>>();

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ReadSynonymWords();
        ReadOppositeWords();
        ReadSingleWords();
    }

    void ReadSynonymWords()
    {
        string fs = m_taSynomymWords.text;
        string[] fLines = Regex.Split(fs, "\n|\r|\r\n");

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];
            int lineLength = valueLine.Length;
            string[] values = Regex.Split(valueLine, ",");

            if (values.Length == 2)
            {
                if (m_synonymAllWords.ContainsKey(lineLength))
                {
                    m_synonymAllWords[lineLength].Add(valueLine);
                }
                else
                {
                    m_synonymAllWords.Add(lineLength, new List<string> { valueLine });
                }
            }
        }
    }

    void ReadOppositeWords()
    {
        string fs = m_taOppositeWords.text;
        string[] fLines = Regex.Split(fs, "\n|\r|\r\n");

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];
            int lineLength = valueLine.Length;
            string[] values = Regex.Split(valueLine, ",");

            if (values.Length == 2)
            {
                if (m_oppositeAllWords.ContainsKey(lineLength))
                {
                    m_oppositeAllWords[lineLength].Add(valueLine);
                }
                else
                {
                    m_oppositeAllWords.Add(lineLength, new List<string> { valueLine });
                }
            }
        }
    }

    void ReadSingleWords()
    {
        string fs = m_taSingleWords.text;
        string[] fLines = Regex.Split(fs, "\n|\r|\r\n");

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];
            int lineLength = valueLine.Length;

            if (m_singleAllWords.ContainsKey(lineLength))
            {
                m_singleAllWords[lineLength].Add(valueLine);
            }
            else
            {
                m_singleAllWords.Add(lineLength, new List<string> { valueLine });
            }
        }
    }

    public Tuple<string, string> GetDoubleWords(GameType gameType)
    {
        string strWords;
        switch (gameType)
        {
            case GameType.Synonym:
                strWords = GetWords(m_synonymAllWords, m_synonymDoneWords);
                break;
            case GameType.Opposite:
                strWords = GetWords(m_oppositeAllWords, m_oppositeDoneWords);
                break;
            default:
                Logger.Log("WordPool:GetDoubleWords", "Game type not implemented");
                strWords = "Game,Over";
                break;
        }
        string[] words = Regex.Split(strWords, ",");
        return new Tuple<string, string>(words[0], words[1]);
    }

    public string GetSingleWord(GameType gameType)
    {
        string strWord;
        switch (gameType)
        {
            case GameType.SingleWord:
                strWord = GetWords(m_singleAllWords, m_singleDoneWords);
                break;
            case GameType.CustomLevel:
                strWord = GetWords(m_singleAllWords, m_singleDoneWords);
                break;
            default:
                Logger.Log("WordPool:GetDoubleWords", "Game type not implemented");
                strWord = "GameOver";
                break;
        }
        return strWord;
    }

    string GetWords(IDictionary<int, List<string>> allWords, IDictionary<int, List<string>>  doneWords)
    {
        Random rnd = new Random();

        foreach (var group in allWords)
        {
            if (doneWords.ContainsKey(group.Key))
            {
                if (doneWords[group.Key].Count < allWords[group.Key].Count)
                {
                    List<string> availableWords = allWords[group.Key].Where(w => (doneWords[group.Key].Contains(w) == false)).ToList();
                    int availableWordCount = availableWords.Count;
                    int wordIndex = rnd.Next(0, availableWordCount);
                    return availableWords[wordIndex];
                }
                continue;
            }
            else
            {
                List<string> availableWords = allWords[group.Key];
                int availableWordCount = availableWords.Count;
                int wordIndex = rnd.Next(0, availableWordCount);
                return availableWords[wordIndex];
            }
        }
        return "Game,Over";
    }

    void OnGameSucceeded(GameType gameType, string word)
    {

    }
}
