using UnityEngine;
using System.Collections;

public abstract class CPopUp : MonoBehaviour
{
    public abstract void Open();
    public abstract void Close();

    public void Open(Animator animator)
    {
        animator.SetTrigger("Open");
        Open();
    }

    public void Close(Animator animator)
    {
        animator.SetTrigger("Close");
        StartCoroutine(CloseMenu());
    }

    IEnumerator CloseMenu()
    {
        yield return new WaitForSeconds(1);
        Close();
    }
}
