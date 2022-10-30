using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [SerializeField] private CPopUp m_activePopUp;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        else
            Instance = this;
    }

    public void OpenPopUp(CPopUp popup)
    {
        if (m_activePopUp != null)
        {
            m_activePopUp.Close();
        }
        m_activePopUp = popup;
        popup.Open(GetComponent<Animator>());
    }

    public void CloseActivePopup()
    {
        if (m_activePopUp != null)
        {
            m_activePopUp.Close(GetComponent<Animator>());
        }
    }
}
