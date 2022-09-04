using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    [SerializeField] private GameObject m_goLoadingScreen;

    [SerializeField] private CMenu m_currentMenu;
    [SerializeField] private CMenu m_playMenu;

    void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(Instance);
        else
            Instance = this;
    }

    public void InitMenu()
    {
        m_goLoadingScreen.SetActive(false);
        gameObject.SetActive(true);
        OpenMenu(m_currentMenu);
    }

    public void OpenPlayMenu(GameState gameState)
    {
        ((PlayMenu)m_playMenu).SetGameState(gameState);
        OpenMenu(m_playMenu);
    }

    public void OpenMenu(CMenu menu)
    {
        if(m_currentMenu != null)
        {
            m_currentMenu.Close();
        }
        m_currentMenu = menu;
        menu.Open();
    }
}
