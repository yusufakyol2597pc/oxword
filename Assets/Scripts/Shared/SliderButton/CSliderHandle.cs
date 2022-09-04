using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CSliderHandle : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform m_rectTransform;
    private CanvasGroup canvasGroup;
    private CSliderButton m_sliderButton;

    public void SetSliderButton(CSliderButton sliderButton)
    {
        m_sliderButton = sliderButton;
    }

    [SerializeField]
    private Canvas m_canvas;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        for (int i = 0; i < WordController.Instance.canvasGroupList.Count; i++)
        {
            WordController.Instance.canvasGroupList[i].blocksRaycasts = false;
            GetComponent<RectTransform>().SetAsLastSibling();
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        m_rectTransform.anchoredPosition += new Vector2(eventData.delta.x / m_canvas.scaleFactor, m_rectTransform.anchoredPosition.y);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        for (int i = 0; i < WordController.Instance.canvasGroupList.Count; i++)
        {
            WordController.Instance.canvasGroupList[i].blocksRaycasts = true;
        }
    }
}
