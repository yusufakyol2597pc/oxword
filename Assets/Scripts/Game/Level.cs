using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level : MonoBehaviour
{
    public static Level Instance;

    [SerializeField] private TextMeshProUGUI m_counterText;
    [SerializeField] private TextMeshProUGUI m_resultText;
    [SerializeField] private TextMeshProUGUI m_levelText;
    [SerializeField] private TextMeshProUGUI m_gainedCoinText;

    private Vector3 m_gainedCoinPos;

    [SerializeField] private Sprite[] m_sprWord1List;
    [SerializeField] private Sprite[] m_sprWord2List;

    [SerializeField] private Sprite[] m_sprWordBg1List;
    [SerializeField] private Sprite[] m_sprWordBg2List;

    [SerializeField] private Button m_nextLevelButton;
    [SerializeField] private GameObject m_goPlayAgainButton;

    [SerializeField] private GameObject m_goBgErrorPage;

    [SerializeField] private int m_iLevel;

    private List<List<string>> m_levelWords;

    private float m_time = 0;
    private int m_counterMaxTime = 20;
    private bool m_isRunning;

    private int m_point = 8;
    public GameState m_gameState;

    private int m_iCustomLevelCounter = 0;

    void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(Instance);
        else
            Instance = this;

        DontDestroyOnLoad(this);
    }

    public void Init(GameState gameState)
    {
        m_levelWords = gameState.m_lWords;
        m_gameState = gameState;

        m_gainedCoinPos = m_gainedCoinText.gameObject.GetComponent<RectTransform>().anchoredPosition;
        ResetAnimatedCoin();

        StartLevel(gameState.m_gameType);
    }

    public void StartLevel(GameType gameType)
    {
        m_time = 0;
        m_isRunning = true;
        m_nextLevelButton.gameObject.SetActive(true);
        m_goPlayAgainButton.SetActive(false);
        m_goBgErrorPage?.SetActive(false);
        m_nextLevelButton.interactable = false;

        int levelIndex = m_gameState.m_iLevel - 1;
        m_resultText.text = m_point + " Puan";
        m_levelText.text = "Level " + m_gameState.m_iLevel;

        int bgColorLen = Constants.BG_COLORS.Length;

        int index = levelIndex % bgColorLen;

        if (gameType == GameType.SingleWord)
        {
            WordController.Instance.Initialize(
                m_levelWords[levelIndex][0],
                Constants.BG_COLORS[index, 0],
                m_sprWord1List[index],
                m_sprWordBg1List[index]
            );
        }
        else
        {
            WordController.Instance.Initialize(
            m_levelWords[levelIndex][0],
            m_levelWords[levelIndex][1],
            Constants.BG_COLORS[index, 0],
            Constants.BG_COLORS[index, 1],
            m_sprWord1List[index],
            m_sprWord2List[index],
            m_sprWordBg1List[index],
            m_sprWordBg2List[index]
            );
        }
    }

    public void OnSucceed()
    {
        WordController.Instance.DisableDragging();
        SoundManager.Instance.PlaySound(SoundType.GameSuccess);
        AnimateGainedCoin();

        m_iCustomLevelCounter++;

        m_isRunning = false;
        var d1 = new Dictionary<string, string> { { "point", m_point.ToString() } };
        m_resultText.text = LanguageManager.Instance.Translate("success_text", d1);
        m_nextLevelButton.interactable = true;
        UserState.Instance.OnCoinEarned(m_point);
        UserState.Instance.OnLevelUp(m_gameState);
    }

    private void OnFail()
    {
        WordController.Instance.DisableDragging();
        m_isRunning = false;
        m_goBgErrorPage.SetActive(true);
        m_resultText.text = LanguageManager.Instance.Translate("fail_text");
        OpenErrorPage();
    }

    void OpenErrorPage()
    {
        WordController.Instance.Cleanup();
        m_nextLevelButton?.gameObject.SetActive(false);
        m_goPlayAgainButton.SetActive(true);
    }

    public void NextLevel()
    {
        Logger.Log("NextLevel", "Clicked next level.");

        WordController.Instance.Cleanup();

        if (m_iCustomLevelCounter >= 5)
        {
            m_iCustomLevelCounter = 0;

            Logger.Log("NextLevel", "Start custom level.");
        }

        StartLevel(m_gameState.m_gameType);
    }

    public void PlayAgain()
    {
        Logger.Log("PlayAgain", "Clicked play again.");
        WordController.Instance.Cleanup();
        StartLevel(m_gameState.m_gameType);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isRunning)
        {
            m_time += Time.deltaTime;
            int remainingTime = m_counterMaxTime - (int)m_time;
            m_counterText.text = "" + remainingTime;
        }
        if ((float)m_counterMaxTime - m_time < 0)
        {
            OnFail();
        }
    }

    void ResetAnimatedCoin()
    {
        m_gainedCoinText.gameObject.SetActive(false);
        m_gainedCoinText.gameObject.GetComponent<RectTransform>().anchoredPosition = m_gainedCoinPos;
    }

    void AnimateGainedCoin()
    {
        float animDuration = 0.4f;
        m_gainedCoinText.text = "+" + m_point;
        m_gainedCoinText.alpha = 1f;
        m_gainedCoinText.gameObject.SetActive(true);

        RectTransform rectTransform = m_gainedCoinText.gameObject.GetComponent<RectTransform>();
        LeanTween.moveY(rectTransform, rectTransform.anchoredPosition.y - 25, animDuration).setOnComplete(() => {
            ResetAnimatedCoin();
        });

        LeanTween.value(gameObject, 1f, 0.5f, animDuration).setOnUpdate((float val) =>
        {
            m_gainedCoinText.alpha = val;
        });
    }
}
