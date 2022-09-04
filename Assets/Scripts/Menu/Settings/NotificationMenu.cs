using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationMenu : CMenu
{
    [SerializeField] private SwitchToggle m_switchToggleHint;
    [SerializeField] private SwitchToggle m_switchToggleNotification;

    public override void Open()
    {
        gameObject.SetActive(true);

        m_switchToggleHint.Init(false);
        m_switchToggleNotification.Init(false);
    }

    public override void Close()
    {
        gameObject.SetActive(false);

        m_switchToggleHint.Cleanup();
        m_switchToggleNotification.Cleanup();
    }
}
