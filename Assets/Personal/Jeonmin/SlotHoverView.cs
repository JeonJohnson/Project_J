using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotHoverView : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameTmp;
    [SerializeField] private TextMeshProUGUI itemInfoTmp;


    public void UpdateInfo(Item item)
    {
        if(item == null) return;
        itemImage.sprite = item.item_sprite;
        itemNameTmp.text = item.item_name;
        itemInfoTmp.text = item.item_description;
    }

    private void Update()
    {
        this.gameObject.transform.position = Input.mousePosition;
    }
}
