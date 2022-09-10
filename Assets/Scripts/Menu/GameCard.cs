using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public delegate void Notify();

public class GameCard : MonoBehaviour
{
    public event Notify GameCardUnlocked;

    [SerializeField] private TextMeshProUGUI m_tmpTitle;
    [SerializeField] private TextMeshProUGUI m_tmpPrice;

    private GameState m_game;

    public void Init(GameState game)
    {
        m_game = game;
        m_tmpTitle.text = LanguageManager.Instance.Translate(game.m_strTitle);
        m_tmpPrice.text = "" + game.m_iPrice;
    }

    public void OnClickCard()
    {
        if (m_game.m_bBlocked)
        {
            UnlockGame();
        }
        else
        {
            MenuController.Instance.OpenPlayMenu(m_game);
        }
    }

    void UnlockGame()
    {
        if (UserState.Instance.GetCoinCount() < m_game.m_iPrice)
        {
            Debug.Log("Not enough coin.");
            return;
        }

        m_game.m_bBlocked = false;
        UserState.Instance.OnCoinSpent(m_game.m_iPrice);
        OnGameCardUnlocked();
    }

    protected virtual void OnGameCardUnlocked()
    {
        UserState.Instance.SaveGame(true);
        GameCardUnlocked?.Invoke();
    }

    void StartGame()
    {
        Debug.Log("StartGame");
    }
}
