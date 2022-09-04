using UnityEngine;
using UnityEngine.UI;
using TMPro;

public delegate void DSwitch();

public class SwitchToggle : MonoBehaviour
{
    public event DSwitch ToggleSwitched;

    [SerializeField] RectTransform uiHandleRectTransform;
    [SerializeField] Color backgroundActiveColor;
    [SerializeField] Color backgroundInactiveColor;
    [SerializeField] TextMeshProUGUI m_text;

    Image backgroundImage;

    Button toggle;

    string m_onText, m_offText;

    bool m_on = false;

    Vector2 handlePosition;

    float m_moveAmount = 100;
    float m_animDuration = 0.15f;

    bool m_isAnimating = false;

    private void Awake()
    {
        handlePosition = uiHandleRectTransform.anchoredPosition;
        toggle = GetComponent<Button>();

        backgroundImage = uiHandleRectTransform.parent.GetComponent<Image>();
    }

    public void Init(bool isOn)
    {
        m_onText = LanguageManager.Instance.Translate("on");
        m_offText = LanguageManager.Instance.Translate("off");

        m_on = isOn;
        float move = 0;
        if (m_on) {
            backgroundImage.color = backgroundActiveColor;
            move = m_moveAmount;
        }
        else
        {
            backgroundImage.color = backgroundInactiveColor;
        }
        uiHandleRectTransform.anchoredPosition = new Vector2(handlePosition.x + move, handlePosition.y);

        toggle.onClick.AddListener(OnSwitch);

        m_text.alignment = m_on ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        m_text.text = m_on ? m_onText : m_offText;
    }

    void OnSwitch()
    {
        if (m_isAnimating)
        {
            return;
        }
        m_isAnimating = true; 

        m_text.text = "";

        float move = m_on ? -m_moveAmount : m_moveAmount;
        LeanTween.moveX(uiHandleRectTransform, uiHandleRectTransform.anchoredPosition.x + move, m_animDuration);

        LeanTween.value(gameObject, m_on ? backgroundActiveColor : backgroundInactiveColor, m_on ? backgroundInactiveColor : backgroundActiveColor, m_animDuration).setDelay(0.03f).setOnUpdate((Color c) =>
        {
            backgroundImage.color = c;

            var tempColor = backgroundImage.color;
            tempColor.a = 1f;
            backgroundImage.color = tempColor;
        }).setOnComplete(() => {
            m_on = !m_on;
            m_text.alignment = m_on ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            m_text.text = m_on ? m_onText : m_offText;
            m_isAnimating = false;
            OnToggleSwitched();
        });
    }

    protected virtual void OnToggleSwitched()
    {
        UserState.Instance.SaveGame();
        ToggleSwitched?.Invoke();
    }

    public void Cleanup()
    {
        toggle.onClick.RemoveAllListeners();
    }

    public bool IsOn()
    {
        return m_on;
    }

    void OnDestroy()
    {
        toggle.onClick.RemoveAllListeners();
    }
}