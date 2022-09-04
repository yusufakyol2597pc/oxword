using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSliderButton : MonoBehaviour
{
    [SerializeField] private CSliderFill m_fill;
    [SerializeField] private CSliderHandle m_handle;

    // Start is called before the first frame update
    void Start()
    {
        m_handle.SetSliderButton(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetFillWidth()
    {
        return 100;
    }

    public float GetFillHeight()
    {
        return 30;
    }
}
