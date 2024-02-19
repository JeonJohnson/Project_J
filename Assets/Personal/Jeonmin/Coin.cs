using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    public int coinCount;
    public float removeTime = 25f;
    [SerializeField] SpriteRenderer itemSpriteRenderer;
    private Suckable suckable;
    private Collider2D col;

    private void OnSuckedEvent()
    {
        IngameController.Instance.Player.inventroy.coinCount.Value += coinCount;
    }

    private IEnumerator ReturnPoolingCenterCoro()
    {
        yield return new WaitForSeconds(removeTime);

        suckable.col.enabled = false;
        suckable.srdr.DOColor(Color.clear, 1f).OnComplete(() => { PoolingManager.Instance.ReturnObj(this.gameObject); });
    }

    public void Return()
    {
        PoolingManager.Instance.ReturnObj(this.gameObject);
    }

    public void PoolableInit()
    {
        suckable = GetComponent<Suckable>();
        col = this.gameObject.GetComponent<Collider2D>();
        col.enabled = true;
        suckable.OnSucked = null;
        suckable.OnSucked += () => OnSuckedEvent();
    }

    public void PoolableReset()
    {
        suckable.srdr.color = Color.white;
    }

    private void OnEnable()
    {
        PoolableInit(); // 이거 안쓸땐 지우기
        StartCoroutine(ReturnPoolingCenterCoro());
    }
}
