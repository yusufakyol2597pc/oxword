using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMenu : CPopUp
{
    [SerializeField] private SwitchToggle m_switchToggleSound;
    [SerializeField] private SwitchToggle m_switchToggleVibration;

    public override void Open()
    {
        gameObject.SetActive(true);

        m_switchToggleSound.Init(UserState.Instance.m_bSounds);
        m_switchToggleVibration.Init(UserState.Instance.m_bVibration);

        m_switchToggleSound.ToggleSwitched += ToggleSound;
        m_switchToggleVibration.ToggleSwitched += ToggleVibration;
    }

    public override void Close()
    {
        m_switchToggleSound.Cleanup();
        m_switchToggleVibration.Cleanup();

        gameObject.SetActive(false);
    }

    void ToggleSound()
    {
        Logger.Log("ToggleSound", "Sound setting toggled: " + m_switchToggleSound.IsOn());

        UserState.Instance.SetSoundIsOn(m_switchToggleSound.IsOn());
    }

    void ToggleVibration()
    {
        Logger.Log("ToggleVibration", "Vibration setting toggled: " + m_switchToggleVibration.IsOn());


        UserState.Instance.SetVibrationIsOn(m_switchToggleVibration.IsOn());
    }
}
