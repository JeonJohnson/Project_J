using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class Enemy_DeadBody : MonoBehaviour, IPoolable
{
    public Item itemData;
    public float removeTime = 10f;

    [SerializeField] SpriteRenderer itemSpriteRenderer;
    private Suckable suckable;
    private Collider2D col;

    public void Init(Sprite sprite)
    {
        col = this.gameObject.GetComponent<Collider2D>();
        suckable = this.gameObject.GetComponent<Suckable>();
        suckable.srdr.sprite = sprite;
    }

    private void OnSuckedEvent()
    {
        if (!itemData)
        {
            Debug.Log("아이템데이터 없음");
            return;
        }

        IngameController.Instance.Player.inventroy.AddItem(itemData);
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
        suckable.col.enabled = true;
        suckable = GetComponent<Suckable>();
        col = this.gameObject.GetComponent<Collider2D>();
        col.enabled = true;
        suckable.OnSucked = null;
        suckable.OnSucked += () => OnSuckedEvent();

        StartCoroutine(ReturnPoolingCenterCoro());
    }

    public void PoolableReset()
    {
        suckable.srdr.color = Color.white;
    }
}
