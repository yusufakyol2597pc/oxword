using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CGameDoubleWord : CGame
{
    [SerializeField] private TextMeshProUGUI m_resultText;
    [SerializeField] private TextMeshProUGUI m_levelText;

    [SerializeField] private Sprite[] m_sprUpperLetterList;
    [SerializeField] private Sprite[] m_sprLowerLetterList;
    [SerializeField] private Sprite[] m_sprUpperLetterBgList;
    [SerializeField] private Sprite[] m_sprLowerLetterBgList;

    [SerializeField] private Image m_upperBgImage;
    [SerializeField] private Image m_lowerBgImage;

    public override void StartGame()
    {
        gameObject.SetActive(true);

        int levelIndex = m_gameState.m_iLevel - 1;
        m_resultText.text = m_point + " " + LanguageManager.Instance.Translate("point");
        m_levelText.text = "Level " + m_gameState.m_iLevel;

        int colorIndex = levelIndex % Constants.BG_COLORS.GetLength(0);
        string upperBgColor = Constants.BG_COLORS[colorIndex, 0];
        string lowerBgColor = Constants.BG_COLORS[colorIndex, 1];

        Color colorBg;
        ColorUtility.TryParseHtmlString(upperBgColor, out colorBg);
        m_upperBgImage.color = colorBg;

        ColorUtility.TryParseHtmlString(lowerBgColor, out colorBg);
        m_lowerBgImage.color = colorBg;

        int sprWordIndex = levelIndex % m_sprUpperLetterList.Length;
        int sprWordBgIndex = levelIndex % m_sprUpperLetterBgList.Length;
        int wordIndex = levelIndex % m_levelWords.Count;
        string upperWord = m_levelWords[wordIndex][0];
        string lowerWord = m_levelWords[wordIndex][1];

        m_counterMaxTime = 10 * ((upperWord.Length + lowerWord.Length) / DURATION_CONSTANT);

        WordManager.Instance.Initialize(
                this,
                upperWord,
                lowerWord,
                m_sprUpperLetterList[sprWordIndex],
                m_sprLowerLetterList[sprWordIndex],
                m_sprUpperLetterBgList[sprWordBgIndex],
                m_sprLowerLetterBgList[sprWordBgIndex]
           );
    }

    public override void OnSucceed()
    {
        WordManager.Instance.DisableDragging();
        PlaySuccessSound();
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
        PlayFailSound();
        m_isRunning = false;
        m_goBgErrorPage.SetActive(true);
        m_resultText.text = LanguageManager.Instance.Translate("fail_text");
        WordManager.Instance.Cleanup();
        m_nextLevelButton?.gameObject.SetActive(false);
        m_goPlayAgainButton.SetActive(true);
    }
}
