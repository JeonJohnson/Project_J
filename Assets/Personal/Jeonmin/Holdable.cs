using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Holdable : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform holdTargetTr;
    public UnityAction OnFired;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(holdTargetTr)
        {
            this.transform.position = holdTargetTr.position;
        }
    }

    public void Hold(Transform targetTr)
    {
        holdTargetTr = targetTr;
    }

    public void Fire(Vector2 dir , float moveSpd = 200f)
    {
        holdTargetTr = null;
        rb.AddForce(dir * moveSpd);
        OnFired?.Invoke();
    }
}
