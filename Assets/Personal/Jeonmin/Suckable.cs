using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Suckable : MonoBehaviour
{
    public struct SuckEvent
    {
        public int Order;
        public SuckEvent(int order)
        {
            Order = order;
        }

        static SuckEvent e;
        public static void Trigger(int order)
        {
            e.Order = order;
            MMEventManager.TriggerEvent(e);
        }
    }

    // sucked option 
    public enum BulletState
    {
        Fire, //�߻縸 ���� ��
        SuckWait, //Suction ���۵ǰ� ��� ��ġ ������ ��
        Sucking, //�� �� �׾Ƹ� �Ա������� ������ ��
        End
    }

    [Serializable]
    public struct SplatterStat
    {
        public Vector2 bulletDir;
        public int maxCount;
        public int leftCount;
        public TextMeshProUGUI hitCountTmp;
    }

    public enum SuckedOption
    {
        None,
        Sucked
    }
    [Serializable]
    public struct SuckedStat
    {
        public float suckWaitRandTime; //���ߴ� �ð� (�����ٲ���)
        public float suckingRandTime; //���߰��� �������� �ɸ��� �ð�. (���� �ٲ���)
        public float suckingTimeRatio;

        public Vector2 suckStartPos;
        public Transform suckedTr;
    }

    [Header("Options")]

    [Tooltip("���� ��������")]
    public SuckedStat suckedStat;

    [Tooltip("���� ����")]
    public BulletState curState;

    [Header("Default Components")]
    private BoxCollider2D col;
    private Rigidbody2D rb;
    public SpriteRenderer srdr;
    public InventoryItem item;
    private Projectile projectile;
    private bool isDistanceLimit;

    private Color defColor;
    private Vector2 defScale;

    private void Awake()
    {
        defColor = srdr.color;
        defScale = this.transform.localScale;

        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Sucked(Transform _suckedTr)
    {
        projectile = this.gameObject.GetComponent<Projectile>();
        if(projectile != null)
        {
            if (projectile.DistanceLimit) isDistanceLimit = true;
            else isDistanceLimit = false;
            projectile.DistanceLimit = false;
            projectile.enabled = false;
        }
        if (rb != null) rb.velocity = Vector3.zero;

        srdr.color = srdr.color * 5f;
        curState = BulletState.SuckWait;
        suckedStat.suckWaitRandTime = UnityEngine.Random.Range(0.1f, 0.25f);

        col.enabled = false;
        suckedStat.suckedTr = _suckedTr;

        StartCoroutine(SuckWaitCor());
    }

    public IEnumerator SuckWaitCor()
    {
        yield return new WaitForSeconds(suckedStat.suckWaitRandTime);

        suckedStat.suckingRandTime = UnityEngine.Random.Range(0.15f, 0.35f);
        suckedStat.suckStartPos = transform.position;

        curState = BulletState.Sucking;
        suckedStat.suckingTimeRatio = 0f;

        while (suckedStat.suckingTimeRatio <= 1f)
        {
            suckedStat.suckingTimeRatio += Time.deltaTime / suckedStat.suckingRandTime;
            transform.position = Vector2.Lerp(suckedStat.suckStartPos, suckedStat.suckedTr.position, suckedStat.suckingTimeRatio);
            srdr.color = new Color(srdr.color.r, srdr.color.g, srdr.color.b, 1 - suckedStat.suckingTimeRatio * 2f);
            this.transform.localScale = new Vector2(1 - suckedStat.suckingTimeRatio, 1 - suckedStat.suckingTimeRatio);
            yield return null;
        }

        //projectile = this.gameObject.GetComponent<Projectile>();
        //if (projectile != null)
        //{
        //    LevelManager.Instance.Players[0].GetComponent<CharacterInventory>().MainInventory.EquipItem(item, 1);
        //}
        //else
        //{
        //    LevelManager.Instance.Players[0].GetComponent<CharacterInventory>().MainInventory.AddItem(item, 1);
        //    item.Equip("Player1");
        //}

        LevelManager.Instance.Players[0].GetComponent<CharacterInventory>().MainInventory.AddItem(item, 1);
        LevelManager.Instance.Players[0].GetComponent<CharacterInventory>().MainInventory.EquipItem(item, 1);
        UpdateAmmoDisplay();
        //item.Equip("Player1");
        SuckEvent.Trigger(1);

        Resetting();
        //�����ϱ�
        this.gameObject.SetActive(false);
    }

    public void Resetting()
    {
        if (projectile != null)
        {
            projectile.DistanceLimit = isDistanceLimit;
            projectile.enabled = true;
        }
        col.enabled = true;
        this.transform.localScale = defScale;
        srdr.color = Color.white;
        curState = BulletState.Fire;
    }

    public virtual void UpdateAmmoDisplay()
    {
        MoreMountains.TopDownEngine.Weapon CurrentWeapon = LevelManager.Instance.Players[0].GetComponent<CharacterHandleWeapon>().CurrentWeapon;
        Debug.Log(CurrentWeapon.WeaponAmmo.CurrentAmmoAvailable + CurrentWeapon.CurrentAmmoLoaded);
        MMInventoryEvent.Trigger(MMInventoryEventType.Pick, null, item.TargetInventoryName, item, 1, 0, LevelManager.Instance.Players[0].PlayerID);
        //GUIManager.Instance.UpdateAmmoDisplays(false, CurrentWeapon.WeaponAmmo.CurrentAmmoAvailable + CurrentWeapon.CurrentAmmoLoaded, CurrentWeapon.CurrentAmmoLoaded, CurrentWeapon.WeaponAmmo.MaxAmmo, CurrentWeapon.MagazineSize, LevelManager.Instance.Players[0].PlayerID, LevelManager.Instance.Players[0].GetComponent<CharacterHandleWeapon>().AmmoDisplayID, false);
    }
}	
