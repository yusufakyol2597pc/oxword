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

    [SerializeField] private GameObject m_goFailX;

    public override void StartGame()
    {
        gameObject.SetActive(true);
        m_goFailX.SetActive(false);

        int levelIndex = m_gameState.m_iLevel - 1;
        m_levelText.text = "Level " + m_gameState.m_iLevel;

        int colorIndex = levelIndex % Constants.BG_COLORS.GetLength(0);
        string bgColor = Constants.BG_COLORS[colorIndex, 0];

        int sprWordIndex = levelIndex % m_sprWordList.Length;
        int sprWordIBgndex = levelIndex % m_sprWordBgList.Length;

        Color colorBg;
        ColorUtility.TryParseHtmlString(bgColor, out colorBg);
        m_BgImage.color = colorBg;

        string word = WordPool.Instance.GetSingleWord(m_gameState.m_gameType);

        int complexity = WordManager.Instance.Initialize(
                this,
                word,
                m_sprWordList[sprWordIndex],
                m_sprWordBgList[sprWordIBgndex]
            );
        m_point = complexity * Constants.POINT_COMPLEXITY_CONSTANT;
        m_counterMaxTime = complexity * Constants.DURATION_COMPLEXITY_CONSTANT;
        m_resultText.text = m_point + " " + LanguageManager.Instance.Translate("point");
        m_isRunning = true;
    }

    public override void OnSucceed()
    {
        WordManager.Instance.DisableDragging();
        PlaySuccessSound();
        AnimateGainedCoin();

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
        m_BgImage.color = Utils.GetColorFromString(Constants.FAIL_SCREEN_BG_COLOR);
        m_goFailX.SetActive(true);
        m_resultText.text = LanguageManager.Instance.Translate("fail_text");
        WordManager.Instance.OnGameFailed();
        m_nextLevelButton?.gameObject.SetActive(false);
        m_goPlayAgainButton.SetActive(true);
    }
}
