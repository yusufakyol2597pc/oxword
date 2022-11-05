using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMenu : CPopUp
{
    [SerializeField] private SwitchToggle m_switchToggleMusic;
    [SerializeField] private SwitchToggle m_switchToggleSoundFx;

    public override void Open()
    {
        gameObject.SetActive(true);

        m_switchToggleMusic.Init(false);
        m_switchToggleSoundFx.Init(false);
    }

    public override void Close()
    {
        gameObject.SetActive(false);

        m_switchToggleMusic.Cleanup();
        m_switchToggleSoundFx.Cleanup();
    }
}
