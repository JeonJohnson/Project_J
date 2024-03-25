using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_ItemHolder : MonoBehaviour, IPoolable
{
    public Item_Rune itemData;

    [SerializeField] SpriteRenderer itemSpriteRenderer;
    private Suckable suckable;
    private Collider2D col;

    private void OnSuckedEvent()
    {
        if (!itemData)
        {
            Debug.Log("아이템데이터 없음");
            return;
        }
        //IngameController.Instance.Player.inventroy.EquipWeapon((Item_Weapon)itemData);
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
    }

    private void Awake()
    {
        CopyScriptableObjectItem();
        itemSpriteRenderer.sprite = itemData.item_sprite;
    }

    private void CopyScriptableObjectItem()
    {
        Item_Rune copyedItem = itemData.Copy<Item_Rune>();
        itemData = copyedItem;
    }

    public void Equip(Player player)
    {
        itemData.Equip(player);
        this.gameObject.SetActive(false);
    }
}
