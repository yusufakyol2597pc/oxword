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

    public override void StartGame()
    {
        gameObject.SetActive(true);

        int levelIndex = m_gameState.m_iLevel - 1;

        int colorIndex = levelIndex % Constants.BG_COLORS.GetLength(0);
        string bgColor = Constants.BG_COLORS[colorIndex, 0];

        int wordIndex = levelIndex % m_levelWords.Count;
        string word = m_levelWords[wordIndex][0];

        m_counterMaxTime = 10 * (word.Length / DURATION_CONSTANT);

        WordManager.Instance.Initialize(
                this,
                word,
                m_sprLetter,
                m_sprLetterBg
            );
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
        m_goBgErrorPage.SetActive(true);
        m_resultText.text = LanguageManager.Instance.Translate("fail_text");
        WordManager.Instance.Cleanup();
        m_nextLevelButton?.gameObject.SetActive(false);
        m_goPlayAgainButton.SetActive(true);
    }
}
