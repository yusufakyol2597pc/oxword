using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Services.CloudSave;
using System.Threading.Tasks;

public class UserState : MonoBehaviour
{
    public static UserState Instance;
    private int m_iCoin = 0;
    private int m_iHint = 0;

    [SerializeField] private MenuController menuController;
    [SerializeField] private GameObject m_goGameCanvas;

    private CGame m_activeGame;
    [SerializeField] CGameSingleWord m_gameSingleWord;
    [SerializeField] CGameDoubleWord m_gameDobuleWord;
    [SerializeField] CGameCustomLevel m_customLevel;

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

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;

        httpHandler = GetComponent<HttpHandler>();

        WordsFileName = Application.persistentDataPath + "/SavedWords.txt";
        UserSaveFileName = Application.persistentDataPath + "/UserSave8.txt";
    }

    // Start is called before the first frame update
    async Task Start()
    {
        Logger.Log("Initialize", "Check if user save file exists.");

        Application.targetFrameRate = 60;
        await Authenticator.Authenticate();

        if (File.Exists(UserSaveFileName))
        {
            await ResumeGame();
        }
        else
        {
            NewGame();
        }
    }

    async Task ResumeGame()
    {
        Logger.Log("ResumeGame", "Read user save from file.");

        StreamReader reader = new StreamReader(UserSaveFileName);
        string strJson = reader.ReadToEnd();
        UserSave userSave = JsonUtility.FromJson<UserSave>(strJson);

        await LoadGame();

        Initialize(userSave);
    }

    void NewGame()
    {
        Logger.Log("NewGame", "Start new game with creating new save file.");

        List<GameState> games = new List<GameState>();

        GameState game1 = new GameState("opposite_game", GameType.Opposite);
        GameState game2 = new GameState("synonym_game", GameType.Synonym);
        GameState game3 = new GameState("single_game", GameType.SingleWord);
        GameState game4 = new GameState("custom_level", GameType.CustomLevel);

        m_lGames = new List<GameState>();
        m_lGames.Add(game1);
        m_lGames.Add(game2);
        m_lGames.Add(game3);
        m_lGames.Add(game4);

        m_iCoin = 60000;
        m_iHint = 5;
        SaveGame(true);

        UserSave userSave = new UserSave();
        userSave.m_bSounds = m_bSounds;
        userSave.m_bVibration = m_bVibration;

        Initialize(userSave);
    }

    void Initialize(UserSave userSave)
    {
        m_bSounds = userSave.m_bSounds;
        m_bVibration = userSave.m_bVibration;

        InitMenu();
    }

    public void SaveGame(bool saveToCloud = true)
    {
        if (saveToCloud == true)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SaveToCloud();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        StartCoroutine(SaveToFile());
    }

    IEnumerator SaveToFile() {
        Logger.Log("SaveToFile", "Save game.");
        yield return 0;

        UserSave userSave = new UserSave();
        userSave.m_bSounds = m_bSounds;
        userSave.m_bVibration = m_bVibration;

        string saveJson = JsonUtility.ToJson(userSave);
        StreamWriter writer = new StreamWriter(UserSaveFileName, false);
        writer.Write(saveJson);
        writer.Flush();
        writer.Close();
    }

    async Task SaveToCloud() {
        Logger.Log("SaveToCloud", "Save game.");

        await SaveData("games", m_lGames);
        await SaveData("coin", m_iCoin);
        await SaveData("hintCount", m_iHint);
    }

    async Task LoadGame()
    {
        Logger.Log("LoadGame", "Load game.");

        m_lGames = await LoadArrayData<GameState>("games");
        m_iCoin = await LoadIntData("coin");
        m_iHint = await LoadIntData("hintCount");
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

    public void StartGame(GameType gameType)
    {
        foreach (GameState gameState in m_lGames)
        {
            if (gameState.m_gameType == gameType)
            {
                StartGame(gameState);
                return;
            }
        }
    }

    public void StartGame(GameState gameState)
    {
        Logger.Log("StartGame", gameState .ToString() + " game started.");

        m_goGameCanvas.SetActive(true);
        LoadLevel(gameState);
    }

    void LoadLevel(GameState gameState)
    {
        m_activeGame?.End();

        switch (gameState.m_gameType)
        {
            case GameType.SingleWord:
                m_gameSingleWord.Startup(gameState);
                m_activeGame = m_gameSingleWord;
                break;
            case GameType.Synonym:
            case GameType.Opposite:
                m_gameDobuleWord.Startup(gameState);
                m_activeGame = m_gameDobuleWord;
                break;
            default:
                Logger.Log("LoadLevel", "Game type not implemented");
                break;
        }
    }

    public void LoadCustomLevel(GameType lastGameType)
    {
        Debug.Log("start custom levellll " + m_lGames.Count);
        foreach (GameState gameState in m_lGames)
        {
            if (gameState.m_gameType == GameType.CustomLevel)
            {
                m_customLevel.Startup(gameState);
                m_customLevel.SetLastGameType(lastGameType);
                break;
            }
        }
    }

    public void OnCoinEarned(int coin)
    {
        m_iCoin += coin;
        SaveGame();
    }

    public void OnCoinSpent(int coin)
    {
        m_iCoin -= coin;
        SaveGame();
    }

    public void OnHintUsed()
    {
        m_iHint--;
        SaveGame();
    }

    public void OnLevelUp(GameState gameState)
    {
        WordPool.Instance.OnGameSucceeded();
        foreach (GameState state in m_lGames)
        {
            if (gameState == state)
            {
                state.m_iLevel += 1;
            }
        }
        SaveGame(true);
    }

    public int GetCoinCount()
    {
        return m_iCoin;
    }

    public int GetHintCount()
    {
        return m_iHint;
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
