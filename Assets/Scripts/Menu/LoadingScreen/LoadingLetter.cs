using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingLetter : MonoBehaviour
{
    [SerializeField] float m_letterIndex;
    private float m_delayBetweenAnims = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayAnim());
    }

    IEnumerator PlayAnim()
    {
        yield return new WaitForSeconds(m_delayBetweenAnims * m_letterIndex);

        GetComponent<Animator>().Play("LoadingAnim");
    }
}
