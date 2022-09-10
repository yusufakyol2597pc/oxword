using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CGameSingleWord : CGame
{
    [SerializeField] private TextMeshProUGUI m_resultText;
    [SerializeField] private TextMeshProUGUI m_levelText;

    [SerializeField] private Sprite[] m_sprWordList;
    [SerializeField] private Sprite[] m_sprWordBgList;

    [SerializeField] private Image m_BgImage;

    public override void StartGame()
    {
        gameObject.SetActive(true);

        int levelIndex = m_gameState.m_iLevel - 1;
        m_resultText.text = m_point + " Puan";
        m_levelText.text = "Level " + m_gameState.m_iLevel;

        int colorIndex = levelIndex % Constants.BG_COLORS.GetLength(0);
        string bgColor = Constants.BG_COLORS[colorIndex, 0];

        int sprWordIndex = levelIndex % m_sprWordList.Length;
        int sprWordIBgndex = levelIndex % m_sprWordBgList.Length;
        int wordIndex = levelIndex % m_levelWords.Count;

        Color colorBg;
        ColorUtility.TryParseHtmlString(bgColor, out colorBg);
        m_BgImage.color = colorBg;

        Debug.Log("cnt: " + m_levelWords.Count + ", " + sprWordIndex + ", " + sprWordIBgndex + ", " + wordIndex);
        WordManager.Instance.Initialize(
                this,
                m_levelWords[levelIndex % m_levelWords.Count][0],
                m_sprWordList[sprWordIndex],
                m_sprWordBgList[sprWordIBgndex]
            );
    }

    public override void OnSucceed()
    {
        WordManager.Instance.DisableDragging();
        base.AnimateGainedCoin();

        s_iCustomLevelCounter++;

        m_isRunning = false;
        var d1 = new Dictionary<string, string> { { "point", m_point.ToString() } };
        m_resultText.text = LanguageManager.Instance.Translate("success_text", d1);
        m_nextLevelButton.interactable = true;
        UserState.Instance.OnCoinEarned(m_point);
        UserState.Instance.OnLevelUp(m_gameState);
    }

    public override void OnFail()
    {
        WordManager.Instance.DisableDragging();
        m_isRunning = false;
        m_goBgErrorPage.SetActive(true);
        m_resultText.text = LanguageManager.Instance.Translate("fail_text");
        OpenErrorPage();
    }

    void OpenErrorPage()
    {
        WordManager.Instance.Cleanup();
        m_nextLevelButton?.gameObject.SetActive(false);
        m_goPlayAgainButton.SetActive(true);
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
}
