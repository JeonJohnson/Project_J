using System;
using System.Collections;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    public Item itemData;

    [SerializeField] SpriteRenderer itemSpriteRenderer;
    [SerializeField] GameObject interactButton;

    private void Awake()
    {
        CopyScriptableObjectItem();
        itemSpriteRenderer.sprite = itemData.item_sprite;
    }

    private void CopyScriptableObjectItem()
    {
        Type itemType = itemData.GetType();
        if (itemType == typeof(Item_Active))
        {
            Item_Active copyedItem = itemData.Copy<Item_Active>();
            itemData = copyedItem;
        }
        else if (itemType == typeof(Item_Passive))
        {
            Item_Passive copyedItem = itemData.Copy<Item_Passive>();
            itemData = copyedItem;
        }
        else
        {
            Item copyedItem = itemData.Copy<Item>();
            itemData = copyedItem;
        }
    }

    public void Equip(Player player)
    {
        itemData.Equip(player);
        this.gameObject.SetActive(false);
    }

    public void ShowInteractButton(bool value)
    {
        interactButton.SetActive(value);
    }

    private bool isStartSucking = false;
    private Vector3 startPosition;
    private float currentTime = 0f;

    public bool Sucking(Player player)
    {
        currentTime += Time.deltaTime * 2f;
        float t = currentTime / 1f;
        if (t >= 1f)
        {
            if(isSuckCoroPlayed == false) StartCoroutine(SuckCoro(player));
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0f, 1f);
    }

    private float suckingTimeRatio;
    private float suckingTime = 1f;
    private bool isSuckCoroPlayed = false;

    IEnumerator SuckCoro(Player player)
    {
        isSuckCoroPlayed = true;
        while (suckingTimeRatio >= 1f)
        {
            suckingTimeRatio += Time.deltaTime / suckingTime;
            transform.position = Vector2.Lerp(startPosition, player.curWeapon.firePos.position, suckingTimeRatio);
            yield return null;
        }
        Equip(player);
        Debug.Log("È£·Î·Ï");
        this.gameObject.SetActive(false);
    } 
}
