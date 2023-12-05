using System;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    public Item itemData;

    [SerializeField] SpriteRenderer itemSpriteRenderer;
    [SerializeField] GameObject interactButton;

    private BulletState state;

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


    private void Update()
    {
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0f, 1f);
    }

    private bool isStartSucking = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float currentTime = 0f;

    public bool Sucking(Player _player)
    {
        startPosition = this.transform.position;
        itemSpriteRenderer.color = Color.black;
        state = BulletState.Sucking;
        startPosition = this.transform.position;
        targetPosition = _player.curWeapon.firePos.position;

        currentTime += Time.deltaTime * 2f;
        float t = currentTime / 1f;
        transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.Clamp01(t));
        if (t >= 1f)
        {
            Equip(_player);
            Debug.Log("È£·Î·Ï");
            this.gameObject.SetActive(false);
            return false;
        }
        else
        {
            return true;
        }
    }
}
