using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Throwable : MonoBehaviour
{
    [SerializeField] private Transform holderTr;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    public bool isAutoReturn;
    public float autoReturnTime;

    public bool isThrow;
    public Data<CObj> triggedCObj;

    public UnityEvent OnThrow;
    public UnityEvent OnReturn;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
        triggedCObj = new Data<CObj> ();
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
        isThrow = true;

        if (isAutoReturn) StartCoroutine(ReturnCoro());
        if (ReturnAnimCoro != null) StopCoroutine(ReturnAnimCoro);
    }

    public void Return()
    {
        col.enabled = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;
        ReturnAnimCoro = StartCoroutine(ReturnAnimIenumerator());
        //this.transform.DOMove(holderTr.position, 1f).OnComplete(() => { this.transform.localPosition = Vector3.zero; });
       // transform.DOLocalRotate(Vector3.zero, 0.25f);
        isThrow = false;
    }

    private Coroutine ReturnAnimCoro;

    private IEnumerator ReturnAnimIenumerator()
    {
        bool isReturned = false;
        while(!isReturned)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, holderTr.position, Time.deltaTime * 3f);
            if(Vector3.Distance(this.transform.position, holderTr.position) < 0.3f)
            {
                isReturned = true;
                transform.parent = holderTr;
                this.transform.localPosition = Vector3.zero;
                transform.DOLocalRotate(Vector3.zero, 0.1f);
            }
            yield return null;
        }
    }

    private IEnumerator ReturnCoro()
    {
        yield return new WaitForSeconds(autoReturnTime);

        Return();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isThrow)
        {
            if(collision.gameObject.GetComponent<CObj>() != null)
            {
                triggedCObj.Value = collision.gameObject.GetComponent<CObj>();
            }
        }
    }
}
