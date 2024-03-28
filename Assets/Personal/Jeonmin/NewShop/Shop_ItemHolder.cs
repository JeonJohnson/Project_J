using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_ItemHolder : Suckable
{
    public Shop owner;
    public Item_Rune itemData;

    [SerializeField] SpriteRenderer itemSpriteRenderer;

    private float suckingTimer = 0f;
    public float suckTime = 2f;
    private float suckCoolTimer = 0f;

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

    public override void PoolableInit()
    {
        base.PoolableInit();
    }

    public override void PoolableReset()
    {
        base.PoolableReset();
        srdr.color = Color.white;
    }

    public void Init(Shop shop, Item_Rune item)
    {
        owner = shop;
        itemData = item;
        srdr.sprite = item.item_sprite;
        OnSucked = null;
        OnSucked += () => OnSuckedEvent();
    }

    private void OnEnable()
    {
        PoolableInit(); // 이거 안쓸땐 지우기
    }

    public override void Awake()
    {
        base.Awake();
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

    public override void Sucking(Transform _suckedTr)
    {
        suckCoolTimer = 0.5f;
        suckingTimer += Time.deltaTime;

        if(suckingTimer >= suckTime)
        {
            if(owner.BuyItem(itemData))
            {
                Sucked(_suckedTr);
            }
            else
            {
                Debug.Log("돈모자람");
            }
        }
    }

    public override void Sucked(Transform _suckedTr)
    {
        base.Sucked(_suckedTr);
    }

    private void Update()
    {
        if(suckCoolTimer < 0f)
        {
            suckingTimer -= Time.deltaTime;
            suckingTimer = Mathf.Clamp(suckingTimer, 0f, 999f);
        }
        else
        {
            suckCoolTimer -= Time.deltaTime;
        }
    }
}
