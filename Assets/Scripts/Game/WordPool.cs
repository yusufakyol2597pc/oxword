using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using Random = System.Random;
using System.IO;
using System.Collections;
using Newtonsoft.Json;

[Serializable]
public class CompletedWords
{
    public IDictionary<int, List<string>> m_synonymDoneWords = new Dictionary<int, List<string>>();
    public IDictionary<int, List<string>> m_oppositeDoneWords = new Dictionary<int, List<string>>();
    public IDictionary<int, List<string>> m_singleDoneWords = new Dictionary<int, List<string>>();
}

public class WordPool : MonoBehaviour
{
    public static WordPool Instance;

    [SerializeField] TextAsset m_taSynomymWords;
    [SerializeField] TextAsset m_taOppositeWords;
    [SerializeField] TextAsset m_taSingleWords;

    [SerializeField] string m_strCurrentWord = "";

    [SerializeField] GameType m_currentGameType;
    [SerializeField] CompletedWords m_completedWords;

    IDictionary<int, List<string>> m_synonymAllWords = new Dictionary<int, List<string>>();
    IDictionary<int, List<string>> m_synonymDoneWords = new Dictionary<int, List<string>>();

    IDictionary<int, List<string>> m_oppositeAllWords = new Dictionary<int, List<string>>();
    IDictionary<int, List<string>> m_oppositeDoneWords = new Dictionary<int, List<string>>();

    IDictionary<int, List<string>> m_singleAllWords = new Dictionary<int, List<string>>();
    IDictionary<int, List<string>> m_singleDoneWords = new Dictionary<int, List<string>>();

    string CompletedWordsFileName = "";

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;

        CompletedWordsFileName = Application.persistentDataPath + "/CompletedWordsFileName1.txt";
    }

    // Start is called before the first frame update
    void Start()
    {
        ReadSynonymWords();
        ReadOppositeWords();
        ReadSingleWords();

        ReadCompletedWords();
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
        m_currentGameType = gameType;
        string strWords;
        switch (gameType)
        {
            case GameType.Synonym:
                strWords = GetWords(m_synonymAllWords, m_completedWords.m_synonymDoneWords);
                break;
            case GameType.Opposite:
                strWords = GetWords(m_oppositeAllWords, m_completedWords.m_oppositeDoneWords);
                break;
            default:
                Logger.Log("WordPool:GetDoubleWords", "Game type not implemented");
                strWords = "Game,Over";
                break;
        }
        m_strCurrentWord = strWords;
        string[] words = Regex.Split(strWords, ",");
        return new Tuple<string, string>(words[0], words[1]);
    }

    public string GetSingleWord(GameType gameType)
    {
        m_currentGameType = gameType;
        string strWord;
        switch (gameType)
        {
            case GameType.SingleWord:
                strWord = GetWords(m_singleAllWords, m_completedWords.m_singleDoneWords);
                break;
            case GameType.CustomLevel:
                strWord = GetWords(m_singleAllWords, m_completedWords.m_singleDoneWords);
                break;
            default:
                Logger.Log("WordPool:GetDoubleWords", "Game type not implemented");
                strWord = "GameOver";
                break;
        }
        m_strCurrentWord = strWord;
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
            }
            else
            {
                Debug.Log(doneWords);
                List<string> availableWords = allWords[group.Key];
                int availableWordCount = availableWords.Count;
                int wordIndex = rnd.Next(0, availableWordCount);
                return availableWords[wordIndex];
            }
        }
        return "Game,Over";
    }

    public void OnGameSucceeded()
    {
        switch (m_currentGameType)
        {
            case GameType.SingleWord:
            case GameType.CustomLevel:
                m_completedWords.m_singleDoneWords = SaveDoneWord(m_completedWords.m_singleDoneWords);
                break;
            case GameType.Synonym:
                m_completedWords.m_synonymDoneWords = SaveDoneWord(m_completedWords.m_synonymDoneWords);
                break;
            case GameType.Opposite:
                m_completedWords.m_oppositeDoneWords = SaveDoneWord(m_completedWords.m_oppositeDoneWords);
                break;
            default:
                Logger.Log("WordPool:OnGameSucceeded", "Game type not implemented");
                break;
        }
        StartCoroutine(SaveCompletedWords());
    }

    IDictionary<int, List<string>> SaveDoneWord(IDictionary<int, List<string>> doneWords)
    {
        int lenWord = m_strCurrentWord.Length;
        if (doneWords.ContainsKey(lenWord))
        {
            doneWords[lenWord].Add(m_strCurrentWord);
            return doneWords;
        }
        doneWords.Add(lenWord, new List<string> { m_strCurrentWord });
        return doneWords;
    }

    void ReadCompletedWords()
    {
        Logger.Log("ReadCompletedWords", "Read completed words from file.");

        StreamReader reader = new StreamReader(CompletedWordsFileName);
        string strJson = reader.ReadToEnd();
#pragma warning disable CS1701 // Assuming assembly reference matches identity
        m_completedWords = JsonConvert.DeserializeObject<CompletedWords>(strJson);
#pragma warning restore CS1701 // Assuming assembly reference matches identity
    }

    IEnumerator SaveCompletedWords()
    {
        Logger.Log("SaveCompletedWords", "SaveCompletedWords");
        yield return 0;

#pragma warning disable CS1701 // Assuming assembly reference matches identity
        string saveJson = JsonConvert.SerializeObject(m_completedWords);
#pragma warning restore CS1701 // Assuming assembly reference matches identity

        StreamWriter writer = new StreamWriter(CompletedWordsFileName, false);
        writer.Write(saveJson);
        writer.Flush();
        writer.Close();
    }
}
