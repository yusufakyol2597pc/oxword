using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CGameCustomLevel : CGame
{
    [SerializeField] private TextMeshProUGUI m_resultText;
    [SerializeField] private TextMeshProUGUI m_levelText;

    [SerializeField] private Sprite m_sprLetter;
    [SerializeField] private Sprite m_sprLetterBg;

    [SerializeField] private Image m_BgImage;
    [SerializeField] private GameObject m_goFailX;
    [SerializeField] private GameObject m_goBgPattern;

    private string BG_COLOR = "#9013FE";

    public override void StartGame()
    {
        gameObject.SetActive(true);
        m_goFailX.SetActive(false);
        m_goBgPattern.SetActive(true);
        m_BgImage.color = Utils.GetColorFromString(BG_COLOR);

        int levelIndex = m_gameState.m_iLevel - 1;

        int colorIndex = levelIndex % Constants.BG_COLORS.GetLength(0);
        string bgColor = Constants.BG_COLORS[colorIndex, 0];

        int wordIndex = levelIndex % m_levelWords.Count;
        string word = m_levelWords[wordIndex][0];

        int complexity = WordManager.Instance.Initialize(
                this,
                word,
                m_sprLetter,
                m_sprLetterBg
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

        m_isRunning = false;
        var d1 = new Dictionary<string, string> { { "point", m_point.ToString() } };
        m_resultText.text = LanguageManager.Instance.Translate("success_text", d1);
        m_nextLevelButton.interactable = true;
        UserState.Instance.OnCoinEarned(m_point);
    }

    public override void OnFail()
    {
        WordManager.Instance.DisableDragging();
        PlayFailSound();
        m_isRunning = false;
        m_resultText.text = LanguageManager.Instance.Translate("fail_text");

        m_BgImage.color = Utils.GetColorFromString(Constants.FAIL_SCREEN_BG_COLOR);
        m_goFailX.SetActive(true);
        m_goBgPattern.SetActive(false);

        WordManager.Instance.OnGameFailed();
        m_nextLevelButton?.gameObject.SetActive(false);
        m_goPlayAgainButton.SetActive(true);
    }
}
