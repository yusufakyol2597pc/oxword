using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System;
using System.IO;
using Unity.Services.CloudSave;
using System.Threading.Tasks;

public class UserState : MonoBehaviour
{
    public static UserState Instance;
    private int m_iCoin = 0;

    [SerializeField] private MenuController menuController;
    [SerializeField] private GameObject m_goGameCanvas;
    [SerializeField] private TextMeshProUGUI m_coinText;

    public List<GameState> m_lGames;
    public bool m_bSounds = true;
    public bool m_bVibration = true;
    public bool m_bMusic;

    private string[,] m_words;
    private string[] m_singleWords;

    private HttpHandler httpHandler;

    string WordsFileName;
    string UserSaveFileName;
    const string addr = "https://blooming-oasis-79912.herokuapp.com/";
    //const string addr = "localhost:3000/";

    void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(Instance);
        else
            Instance = this;

        httpHandler = GetComponent<HttpHandler>();

        WordsFileName = Application.persistentDataPath + "/SavedWords.txt";
        UserSaveFileName = Application.persistentDataPath + "/UserSave.txt";
    }

    // Start is called before the first frame update
    async void Start()
    {
        Logger.Log("Initialize", "Check if user save file exists.");

        await Authenticator.Authenticate();

        if (File.Exists(UserSaveFileName))
        {
            ResumeGame();
        }
        else
        {
            NewGame();
        }
    }

    void ResumeGame()
    {
        Logger.Log("ResumeGame", "Read user save from file.");

        StreamReader reader = new StreamReader(UserSaveFileName);
        string strJson = reader.ReadToEnd();
        UserSave userSave = JsonUtility.FromJson<UserSave>(strJson);

        Initialize(userSave);
    }

    async void NewGame()
    {
        Logger.Log("NewGame", "Start new game with creating new save file.");

        List<GameState> games = new List<GameState>();

        GameState game1 = new GameState("opposite_game", GameType.Opposite);
        GameState game2 = new GameState("synonym_game", GameType.Synonym);
        GameState game3 = new GameState("single_game", GameType.SingleWord);

        m_lGames = new List<GameState>();
        m_lGames.Add(game1);
        m_lGames.Add(game2);
        m_lGames.Add(game3);

        m_iCoin = 60000;
        UserSave userSave = SaveGame();

        await SaveToCloudSynchronous();

        Initialize(userSave);
    }

    void Initialize(UserSave userSave)
    {
        m_lGames = new List<GameState>();
        m_bSounds = userSave.m_bSounds;
        m_bVibration = userSave.m_bVibration;

        LoadArrayData<GameState>("games").ContinueWith(t => {
            m_lGames = t.Result;
        });

        LoadIntData("coin").ContinueWith(t => {
            m_iCoin = t.Result;
            m_coinText.text = "" + t.Result;
        });

        StartCoroutine(GetWords());
    }

    public UserSave SaveGame()
    {
        SaveToCloud();
        return SaveToFile();
    }

    UserSave SaveToFile() {
        Logger.Log("SaveToFile", "Save game.");

        UserSave userSave = new UserSave();
        userSave.m_bSounds = m_bSounds;
        userSave.m_bVibration = m_bVibration;

        string saveJson = JsonUtility.ToJson(userSave);
        StreamWriter writer = new StreamWriter(UserSaveFileName, false);
        writer.Write(saveJson);
        writer.Flush();
        writer.Close();
        return userSave;
    }

    void SaveToCloud() {
        Logger.Log("SaveToCloud", "Save game.");

        SaveData("games", m_lGames);
        SaveData("coin", m_iCoin);
    }

    async Task SaveToCloudSynchronous() {
        Logger.Log("SaveToCloud", "Save game.");

        await SaveData("games", m_lGames);
        await SaveData("coin", m_iCoin);
    }

    public async Task SaveData<T>(string key, T value)
    {
        var data = new Dictionary<string, object>{{key, value}};
        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
    }

    public async Task<int> LoadIntData(string key)
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{key});
        return int.Parse(savedData[key]);
    }

    public async Task<List<T>> LoadArrayData<T>(string key)
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{key});
        string value = savedData[key];
        return JsonHelper.FromJson<T>(value);
    }

    void InitMenu()
    {
        m_goGameCanvas.SetActive(false);
        menuController.InitMenu();
    }

    public void StartGame(GameState gameState)
    {
        Logger.Log("StartGame", gameState .ToString() + " game started.");

        m_goGameCanvas.SetActive(true);
        LoadLevel(gameState);
    }

    void LoadLevel(GameState gameState)
    {
        Level.Instance.Init(gameState);
    }

    public void OnCoinEarned(int coin)
    {
        m_iCoin += coin;
        m_coinText.text = "" + m_iCoin;
        SaveGame();
    }

    public void OnCoinSpent(int coin)
    {
        m_iCoin -= coin;
        m_coinText.text = "" + m_iCoin;
        SaveGame();
    }

    public void OnLevelUp(GameState gameState)
    {
        foreach (GameState state in m_lGames)
        {
            if (gameState == state)
            {
                state.m_iLevel += 1;
            }
        }
        SaveGame();
    }

    public int GetCoinCount()
    {
        return m_iCoin;
    }

    void ReadWords()
    {
        Logger.Log("ReadWords", "Read words from file.");

        StreamReader reader = new StreamReader(WordsFileName);
        string strJson = reader.ReadLine();
        ParseWords(strJson, true);
        InitMenu();
    }

    public IEnumerator GetWords()
    {
        const string method = "GetWords";
        int savedWordCount = PlayerPrefs.GetInt("SavedWordCount", 0);

        UnityWebRequest www = UnityWebRequest.Get(addr + "get-word-count");
        yield return www.SendWebRequest();

        int wordCountInCloud = Int32.Parse(www.downloadHandler.text);
        Logger.Log(method, "Word count in local: " + savedWordCount + ", word count in cloud: " + wordCountInCloud);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Logger.Log(method, www.error);
        }
        else if (wordCountInCloud != savedWordCount)
        {
            StartCoroutine(FetchWords(wordCountInCloud));
        }
        else if (File.Exists(WordsFileName))
        {
            ReadWords();
        }
        else
        {
            StartCoroutine(FetchWords(wordCountInCloud));
        }
    }

    IEnumerator FetchWords(int wordCountInCloud)
    {
        const string METHOD = "FetchWords";
        Logger.Log(METHOD, "Fetch words from mongodb.");

        IWordArray wordArray = new IWordArray();
        wordArray.words = new List<IWord>();
        int page = 1;
        int nWordPerPage = 0;
        while(true)
        {
            UnityWebRequest www = UnityWebRequest.Get(addr + "get-words?page=" + page);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Logger.Log(METHOD, www.error);
            }
            else
            {
                bool isFirstPage = page == 1;
                List<IWord> gotWords = ParseWords(www.downloadHandler.text, !isFirstPage);
                wordArray.words.AddRange(gotWords);

                if (gotWords.Count == 0 || (nWordPerPage != 0 && gotWords.Count < nWordPerPage))
                {
                    break;
                }
                nWordPerPage = gotWords.Count;
            }
            page++;
        }

        InitMenu();
        PlayerPrefs.SetInt("SavedWordCount", wordCountInCloud);
        StreamWriter writer = new StreamWriter(WordsFileName, false);
        writer.Write(JsonUtility.ToJson(wordArray));
        writer.Flush();
        writer.Close();
    }

    List<IWord> ParseWords(string strJson, bool append)
    {
        if (append == false)
        {
            foreach (GameState gameState in m_lGames)
            {
                if (gameState.m_lWords != null)
                {
                    gameState.m_lWords.Clear();
                }
            }
        }

        IWordArray wordArray = IWordArray.ParseJson(strJson);
        foreach (IWord word in wordArray.words)
        {
            foreach (GameState gameState in m_lGames)
            {
                if (gameState.m_gameType.ToString() == word.type)
                {
                    List<string> words = new List<string>();
                    foreach (string strWord in word.words)
                    {
                        words.Add(strWord);
                    }
                    if (gameState.m_lWords == null)
                    {
                        gameState.m_lWords = new List<List<string>>();
                    }
                    gameState.m_lWords.Add(words);
                }
            }
        }
        return wordArray.words;
    }

    public void SetSoundIsOn(bool on)
    {
        m_bSounds = on;
        SaveGame();
    }

    public void SetVibrationIsOn(bool on)
    {
        m_bVibration = on;
        SaveGame();
    }

    public void DeleteSaveFile()
    {
        File.Delete(UserSaveFileName);
        Application.Quit();
    }
}
