using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class CGame : MonoBehaviour
{
    public static readonly int DURATION_CONSTANT = 3;

    [SerializeField] protected TextMeshProUGUI m_counterText;
    [SerializeField] protected TextMeshProUGUI m_gainedCoinText;
    [SerializeField] protected TextMeshProUGUI m_coinText;
    [SerializeField] protected TextMeshProUGUI m_hintCountText;

    private Vector3 m_gainedCoinPos;

    [SerializeField] protected Button m_nextLevelButton;
    [SerializeField] protected GameObject m_goPlayAgainButton;

    [SerializeField] private int m_iLevel;

    protected List<List<string>> m_levelWords;

    protected float m_time = 0;
    protected int m_counterMaxTime = 20;
    protected bool m_isRunning;

    protected int m_point = 0;
    public GameState m_gameState;

    protected static int s_iCustomLevelCounter = 0;

    protected GameType m_lastGameType;

    public void Startup(GameState gameState)
    {
        m_levelWords = gameState.m_lWords;
        m_gameState = gameState;

        m_gainedCoinPos = m_gainedCoinText.gameObject.GetComponent<RectTransform>().anchoredPosition;
        ResetAnimatedCoin();

        PrepareStartGame();
    }

    public void PrepareStartGame()
    {
        m_time = 0;
        m_nextLevelButton.gameObject.SetActive(true);
        m_goPlayAgainButton.SetActive(false);
        m_nextLevelButton.interactable = false;
        m_hintCountText.text = UserState.Instance.GetHintCount().ToString();

        StartGame();
    }

    public void PlayAgain()
    {
        Logger.Log("PlayAgain", "Clicked play again.");
        PrepareStartGame();
    }

    public void NextLevel()
    {
        CloseGame();
        Logger.Log("NextLevel", "Clicked next level.");

        if (m_gameState.m_gameType == GameType.CustomLevel)
        {
            UserState.Instance.StartGame(m_lastGameType);
            return;
        }
        else if (s_iCustomLevelCounter >= Constants.CUSTOM_LEVEL_INTERVAL)
        {
            s_iCustomLevelCounter = 0;
            Logger.Log("NextLevel", "Start custom level.");
            UserState.Instance.LoadCustomLevel(m_gameState.m_gameType);
            return;
        }

        PrepareStartGame();
    }

    public abstract void StartGame();

    public abstract void OnSucceed();

    public abstract void OnFail();

    protected void AnimateGainedCoin()
    {
        StartCoroutine(AnimateGainedCoinInternal());
    }

    IEnumerator AnimateGainedCoinInternal()
    {
        yield return new WaitForSeconds(1.5f);

        SoundManager.Instance.PlaySound(SoundType.CoinCount);
        float animDuration = 0.4f;
        m_gainedCoinText.text = "+" + m_point;
        m_gainedCoinText.alpha = 1f;
        m_gainedCoinText.gameObject.SetActive(true);

        RectTransform rectTransform = m_gainedCoinText.gameObject.GetComponent<RectTransform>();
        LeanTween.moveY(rectTransform, rectTransform.anchoredPosition.y - 25, animDuration).setOnComplete(ResetAnimatedCoin);

        LeanTween.value(gameObject, 1f, 0.5f, animDuration).setOnUpdate((float val) =>
        {
            m_gainedCoinText.alpha = val;
        });
    }

    void ResetAnimatedCoin()
    {
        m_coinText.text = "" + UserState.Instance.GetCoinCount();
        m_gainedCoinText.gameObject.SetActive(false);
        m_gainedCoinText.gameObject.GetComponent<RectTransform>().anchoredPosition = m_gainedCoinPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isRunning)
        {
            m_time += Time.deltaTime;
            int remainingTime = m_counterMaxTime - (int)m_time;
            m_counterText.text = "" + remainingTime;

            if ((float)m_counterMaxTime - m_time < 0)
            {
                m_time = 0;
                m_isRunning = false;
                OnFail();
            }
        }
    }

    public void End()
    {
        gameObject.SetActive(false);
    }

    public void UseHint()
    {
        if (UserState.Instance.GetHintCount() <= 0)
        {
            Logger.Log("UseHint", "Hint count is not enough!");
            return;
        }
        WordManager.Instance.UseHint();
        UserState.Instance.OnHintUsed();
        m_hintCountText.text = UserState.Instance.GetHintCount().ToString();
    }

    protected void PlaySuccessSound()
    {
        StartCoroutine(PlaySuccessSoundInternal());
    }

    IEnumerator PlaySuccessSoundInternal()
    {
        yield return new WaitForSeconds(0.3f);
        SoundManager.Instance.PlaySound(SoundType.GameSuccess);
    }

    protected void PlayFailSound()
    {
        SoundManager.Instance.PlaySound(SoundType.GameFailure);
    }

    public bool IsGameRunning()
    {
        return m_isRunning;
    }

    void CloseGame()
    {
        gameObject.SetActive(false);
    }
}
