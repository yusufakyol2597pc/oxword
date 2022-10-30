using UnityEngine;

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
        Close();
    }
}
