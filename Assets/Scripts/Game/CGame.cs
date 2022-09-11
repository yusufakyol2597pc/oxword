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

    private Vector3 m_gainedCoinPos;

    [SerializeField] protected Button m_nextLevelButton;
    [SerializeField] protected GameObject m_goPlayAgainButton;

    [SerializeField] protected GameObject m_goBgErrorPage;

    [SerializeField] private int m_iLevel;

    protected List<List<string>> m_levelWords;

    protected float m_time = 0;
    protected int m_counterMaxTime = 20;
    protected bool m_isRunning;

    protected int m_point = 8;
    public GameState m_gameState;

    protected static int s_iCustomLevelCounter = 0;

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
        m_isRunning = true;
        m_nextLevelButton.gameObject.SetActive(true);
        m_goPlayAgainButton.SetActive(false);
        m_goBgErrorPage?.SetActive(false);
        m_nextLevelButton.interactable = false;

        StartGame();
    }

    public void PlayAgain()
    {
        Logger.Log("PlayAgain", "Clicked play again.");
        WordManager.Instance.Cleanup();
        PrepareStartGame();
    }

    public void NextLevel()
    {
        Logger.Log("NextLevel", "Clicked next level.");

        WordManager.Instance.Cleanup();

        if (s_iCustomLevelCounter >= 1)
        {
            s_iCustomLevelCounter = 0;
            Logger.Log("NextLevel", "Start custom level.");
            UserState.Instance.LoadCustomLevel();
            return;
        }

        PrepareStartGame();
    }

    public abstract void StartGame();

    public abstract void OnSucceed();

    public abstract void OnFail();

    protected void AnimateGainedCoin()
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
        }
        if ((float)m_counterMaxTime - m_time < 0)
        {
            OnFail();
        }
    }

    public void End()
    {
        gameObject.SetActive(false);
    }
}
