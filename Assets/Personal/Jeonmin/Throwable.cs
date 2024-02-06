using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Throwable : MonoBehaviour
{
    [SerializeField] private Transform holderTr;
    private Rigidbody2D rb;
    private BoxCollider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Throw(5f);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Return();
        }
    }

    public void Throw(float power)
    {
        col.enabled = true;
        transform.parent = null;
        rb.simulated = true;
        rb.AddForce(transform.up * power, ForceMode2D.Impulse);
    }

    public void Return()
    {
        transform.parent = holderTr;
        col.enabled = false;
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        this.transform.DOJump(holderTr.position, 1f, 1, 0.25f);
        transform.DOLocalRotate(Vector3.zero, 0.25f);
    }
}
