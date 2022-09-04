using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayMenu : CMenu
{
    private GameState m_gameState;
    [SerializeField] private TextMeshProUGUI m_levelText;

    public override void Open()
    {
        gameObject.SetActive(true);
        m_levelText.text = "Level " + m_gameState.m_iLevel;
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetGameState(GameState gameState)
    {
        m_gameState = gameState;
    }

    public void StartGame()
    {
        Logger.Log("PlayMenu", "StartGame");
        UserState.Instance.StartGame(m_gameState);
    }
}
