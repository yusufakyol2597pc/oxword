using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public GameObject m_item;
    [SerializeField] Sprite m_inactiveFrame;
    [SerializeField] Sprite m_activeFrame;
    [SerializeField] Image m_frame;

    private int m_letterIndex = -1;

    private Sprite m_sprBg;
    private Sprite m_sprWord;

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            GameObject tempItem = m_item;
            eventData.pointerDrag.GetComponent<Item>().m_itemSlot.GetComponent<ItemSlot>().SetItem(m_item, true, true);
            if (m_item != null)
            {
                SetItem(eventData.pointerDrag, true, true);
            }
        }
    }

    public void SetConfig(Sprite sprBg, Sprite sprWord, int index)
    {
        m_sprBg = sprBg;
        m_sprWord = sprWord;

        GetComponent<Image>().sprite = sprBg;
        if (m_item != null)
        {
            m_item.GetComponent<Image>().sprite = sprWord;
        }

        m_letterIndex = index;
    }

    public Sprite GetSprBg()
    {
        return m_sprBg;
    }

    public Sprite GetSprWord()
    {
        return m_sprWord;
    }

    public void SetItem(GameObject itemGO, bool evaluate = false, bool setFrame = false)
    {
        m_item = itemGO;
        m_item.GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, GetComponent<RectTransform>().anchoredPosition.y + 5);
        m_item.GetComponent<Item>().m_itemSlot = gameObject;
        m_item.GetComponent<RectTransform>().SetAsLastSibling();

        if (m_sprWord != null)
        {
            m_item.GetComponent<Image>().sprite = m_sprWord;
        }

        if (setFrame)
        {
            StartCoroutine(SetActiveFrameEnumerator());
        }

        if (evaluate)
        {
            WordController.Instance.EvaluateMove();
        }
    }

    IEnumerator SetActiveFrameEnumerator()
    {
        yield return new WaitForSeconds(0.05f);
        SetActiveFrame();
        yield return new WaitForSeconds(0.8f);
        ResetFrame();
    }

    public string GetLetter()
    {
        return m_item.GetComponent<Item>().GetLetter();
    }

    public bool CheckLetter(string word)
    {
        if (m_letterIndex == -1)
        {
            return false;
        }
        string letter = word.ToUpper().Substring(m_letterIndex, 1);
        if (m_item != null && m_item.GetComponent<Item>().GetLetter() == letter)
        {
            return true;
        }
        return false;
    }

    public void SetActiveFrame()
    {
        m_frame.gameObject.SetActive(true);
        m_frame.GetComponent<Image>().sprite = m_activeFrame;
    }

    public void SetInactiveFrame()
    {
        m_frame.gameObject.SetActive(true);
        m_frame.GetComponent<Image>().sprite = m_inactiveFrame;
    }

    public void ResetFrame()
    {
        m_frame.gameObject.SetActive(false);
        m_item.GetComponent<Item>().SetAlpha(1f);
    }
}
