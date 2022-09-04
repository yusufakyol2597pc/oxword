using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseGameController : CMenu
{
    [SerializeField] private Transform m_trOpenCardTemplate;
    [SerializeField] private Transform m_trLockedCardTemplate;
    [SerializeField] private Transform m_trCardContainer;
    private List<Transform> m_lCards;

    public override void Open()
    {
        gameObject.SetActive(true);
        m_lCards = new List<Transform>();
        CreateCards();
    }

    public override void Close()
    {
        ClearContainer();
        gameObject.SetActive(false);
    }

    void RefreshScreen()
    {
        CreateCards();
    }

    void CreateCards()
    {
        ClearContainer();

        List<GameState> games = UserState.Instance.m_lGames;

        m_trOpenCardTemplate.gameObject.SetActive(false);
        m_trLockedCardTemplate.gameObject.SetActive(false);

        foreach (GameState game in games)
        {
            Transform cardTransform;
            if (game.m_bBlocked) {
                cardTransform = Instantiate(m_trLockedCardTemplate, m_trCardContainer);
            }
            else {
                cardTransform = Instantiate(m_trOpenCardTemplate, m_trCardContainer);
            }
            cardTransform.gameObject.SetActive(true);
            m_lCards.Add(cardTransform);

            GameCard gameCard = cardTransform.GetComponent<GameCard>();
            gameCard.GameCardUnlocked += RefreshScreen;
            gameCard.Init(game);
        }
    }

    void ClearContainer()
    {
        if (m_lCards == null)
        {
            return;
        }
        foreach (Transform trCard in m_lCards)
        {
            Destroy(trCard.gameObject);
        }
        m_lCards.Clear();
    }

    public void ResetUserSave()
    {
        UserState.Instance.DeleteSaveFile();
    }
}
