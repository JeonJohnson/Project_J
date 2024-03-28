using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Capsule : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Shop_ItemHolder itemHolder;
    public Transform itemPos;

    public void Open()
    {
        animator.SetBool("Open", true);
    }

    public void Close()
    {
        animator.SetBool("Open", false);
    }

    public void Show()
    {
        animator.SetBool("Show", true);
    }
    public void Hide()
    {
        animator.SetBool("Show", false );
    }
}
