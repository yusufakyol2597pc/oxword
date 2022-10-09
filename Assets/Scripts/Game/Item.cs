using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameObject m_itemSlot;
    private RectTransform m_rectTransform;
    private CanvasGroup canvasGroup;
    private string m_letter = "0";
    private bool m_bIslocked = false;

    [SerializeField]
    private Canvas m_canvas;
    
    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetLetter(string letter)
    {
        m_letter = letter.ToUpper();
        transform.Find("Letter").GetComponent<TMPro.TextMeshProUGUI>().text = m_letter;
    }

    public string GetLetter()
    {
        return m_letter;
    }

    public void Lock()
    {
        m_bIslocked = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        WordManager.Instance.OnStartDrag();
        SoundManager.Instance.PlaySound(SoundType.SelectLetter);
        canvasGroup.alpha = 0.6f;
        for(int i = 0; i < WordManager.Instance.canvasGroupList.Count; i++)
        {
            WordManager.Instance.canvasGroupList[i].blocksRaycasts = false;
            GetComponent<RectTransform>().SetAsLastSibling();
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (m_bIslocked)
        {
            return;
        }
        m_rectTransform.anchoredPosition += eventData.delta / m_canvas.scaleFactor;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        WordManager.Instance.OnEndDrag();
        canvasGroup.alpha = 1f;
        if (m_bIslocked)
        {
            return;
        }
        canvasGroup.blocksRaycasts = true;
        for (int i = 0; i < WordManager.Instance.canvasGroupList.Count; i++)
        {
            WordManager.Instance.canvasGroupList[i].blocksRaycasts = true;
        }

        ItemSlot itemSlot = m_itemSlot.GetComponent<ItemSlot>();
        if (itemSlot.m_item != null)
        {
            if (itemSlot.m_item.GetComponent<Item>() == this)
            {
                itemSlot.SetItem(gameObject, false, true);
                SoundManager.Instance.PlaySound(SoundType.DropLetter);
            }
        } 
    }

    public void SetAlpha(float val)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = val;
        }
    }
}
