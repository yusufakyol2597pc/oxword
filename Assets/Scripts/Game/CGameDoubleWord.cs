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

    [SerializeField] private GameObject m_goFailX;

    public override void StartGame()
    {
        gameObject.SetActive(true);
        m_goFailX.SetActive(false);

        int levelIndex = m_gameState.m_iLevel - 1;
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

        int complexity = WordManager.Instance.Initialize(
                this,
                upperWord,
                lowerWord,
                m_sprUpperLetterList[sprWordIndex],
                m_sprLowerLetterList[sprWordIndex],
                m_sprUpperLetterBgList[sprWordBgIndex],
                m_sprLowerLetterBgList[sprWordBgIndex]
           );

        m_point = complexity * Constants.POINT_COMPLEXITY_CONSTANT;
        m_counterMaxTime = complexity * Constants.DURATION_COMPLEXITY_CONSTANT;
        m_resultText.text = m_point + " " + LanguageManager.Instance.Translate("point");
        m_isRunning = true;

        Logger.Log("StartGame with " + m_counterMaxTime + " max time");
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
        m_resultText.text = LanguageManager.Instance.Translate("fail_text");

        m_upperBgImage.color = Utils.GetColorFromString(Constants.FAIL_SCREEN_BG_COLOR);
        m_lowerBgImage.color = Utils.GetColorFromString(Constants.FAIL_SCREEN_BG_COLOR);
        m_goFailX.SetActive(true);

        WordManager.Instance.OnGameFailed();
        m_nextLevelButton?.gameObject.SetActive(false);
        m_goPlayAgainButton.SetActive(true);
    }
}
