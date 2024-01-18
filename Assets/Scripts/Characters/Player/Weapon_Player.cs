using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using Unity.VisualScripting;
using Enums;
using UnityEngine.UI;

public class Weapon_Player : Weapon
{
    [System.Serializable]
    public struct SuctionStat
    {
        public Color fovIdleColor;
        public Color fovSuctionColor;
        public Data<float> curSuctionRatio;
        public float maxSuctionTime;
        public float rechargeTime;
        public float suctionAngle;
        public float suctionRange;

        public LayerMask targetLayer;
    }

    private Player owner;
    public SuctionStat suctionStat;
    public SpriteRenderer fovSprite;

    public AttackMode curAttackMode;

    public Holdable holdableItem;

    public SpriteRenderer weaponSpriteRenderer;

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            fovSprite.material.SetFloat("_ArcAngle", suctionStat.suctionAngle);
            fovSprite.transform.localScale = new Vector2(suctionStat.suctionRange * 2, suctionStat.suctionRange * 2);
        }
        if(owner.aimController != null)
        {
            if(owner.aimController.GetAimAngle(Vector3.up) < 90f)
            {
                ChangeImageSortOrder(weaponSpriteRenderer,"Characters", 0);
                ChangeImageSortOrder(fovSprite, "Characters", 0);
            }
            else
            {
                ChangeImageSortOrder(weaponSpriteRenderer,"Characters", 2);
                ChangeImageSortOrder(fovSprite, "Characters", 2);
            }

        }

        CheckAttackMode();
    }

    public override void Init(CObj _owner)
    {
        base.Init(_owner);
        owner = (Player) _owner;
        suctionStat.curSuctionRatio = new Data<float>();
        suctionStat.curSuctionRatio.Value = 1f;
        fovSprite.material.SetFloat("_ArcAngle", suctionStat.suctionAngle);
        fovSprite.transform.localScale = new Vector2(suctionStat.suctionRange * 2, suctionStat.suctionRange * 2);
    }

    public void CheckAttackMode()
    {
        WeaponData weaponData = owner.inventroy.curWeaponSlot.weaponData;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            curAttackMode = AttackMode.Suck;
        }
        else if (CheckFireType(weaponData.fireTriggerType, KeyCode.Mouse0))
        {
            curAttackMode = AttackMode.Fire;
        }
        else
        {
            curAttackMode = AttackMode.Idle;
        }
    }

    public void CheckFire()
    {
        if(curAttackMode == AttackMode.Fire)
        {
            Fire();
        }
    }

    public override void Fire()
    {
        if (holdableItem != null)
        { 
            holdableItem.Fire(firePos.up, 800);
            owner.attackController.isSuckPossible = true;
            holdableItem = null;
            return;
        }

        if (owner.inventroy.bulletCount.Value <= 0) return;

        WeaponData weaponData = owner.inventroy.curWeaponSlot.weaponData;

        // ���� üũ
        float spreadAngle = weaponData.spread;
        //spreadAngle = CheckSpreadAngle(weaponData, spreadAngle);

        // �Ѿ˰��� üũ
        int bulletNum = weaponData.bulletNumPerFire;

        // �Ѿ˼ӵ� üũ
        float bulletSpeed = weaponData.bulletSpeed;

        // �Ѿ� ũ�� üũ
        float bulletSize = weaponData.bulletSize;

        // �Ѿ� ������ üũ

        int dmg = Mathf.CeilToInt(weaponData.damage);

        float criticalValue = weaponData.critical;
        criticalValue = Mathf.Clamp(criticalValue, 0, 100);
        int rndValue = Random.Range(0, 100);
        if (rndValue < criticalValue) dmg = dmg * 2;

        // �Ѿ����� üũ & �߻�

        for (int i = 0; i < bulletNum; i++)
        {
            if (owner.inventroy.bulletCount.Value <= 0) return;
            GameObject bullet = CheckBulletType(weaponData);
            bullet.transform.position = firePos.transform.position;
            float rnd = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Quaternion rndRot = Quaternion.Euler(0f, 0f, rnd);
            Vector2 rndDir = rndRot * firePos.up;
            rndDir.Normalize();
            bullet.GetComponent<Bullet>().Fire(rndDir, 5, bulletSpeed, bulletSize, dmg);
            //bullet.GetComponent<Projectile>().Speed = bulletSpeed;
            //bullet.GetComponent<Projectile>().SetDirection(rndDir,transform.rotation * Quaternion.Euler(0, 0, -90), false);
            owner.inventroy.bulletCount.Value--;
        }
        fireTimer = weaponData.fireRate;
        fireTimer = CheckFireTimer(weaponData, fireTimer);
        #region OldCode
        //fireTimer -= Time.deltaTime;
        //fireTimer = Mathf.Clamp(fireTimer, 0, stat.fireRate);

        //if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    if (fireTimer <= 0)
        //    {
        //        //��
        //        GameObject bullet = Instantiate(testBulletPrefab);
        //        bullet.transform.position = firePos.position;
        //        bullet.GetComponent<Bullet>().Fire(firePos.up, 5);
        //        fireTimer = stat.fireRate;
        //    }
        //}
        #endregion
    }

    private bool CheckFireType(FireTriggerType triggerType, KeyCode keyCode)
    {
        WeaponData weaponData = owner.inventroy.curWeaponSlot.weaponData;

        switch (triggerType) 
        {
            case FireTriggerType.Normal:
                fireTimer -= Time.deltaTime;
                fireTimer = Mathf.Clamp(fireTimer, 0, weaponData.fireRate);
                if (Input.GetKeyDown(KeyCode.Mouse0) && fireTimer <= 0) return true; else return false;
            case FireTriggerType.Rapid:
                fireTimer -= Time.deltaTime;
                fireTimer = Mathf.Clamp(fireTimer, 0, weaponData.fireRate);
                if (Input.GetKey(KeyCode.Mouse0) && fireTimer <= 0) return true; else return false;
            case FireTriggerType.Charge:
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    fireTimer -= Time.deltaTime;
                    fireTimer = Mathf.Clamp(fireTimer, 0, weaponData.fireRate);
                    if (fireTimer <= 0) return true;
                    else return false;
                }
                else 
                { 
                    fireTimer = weaponData.fireRate;
                    return false;
                }
        }
        return false;
    }

    private float CheckSpreadAngle(WeaponData weaponData, float curSpread)
    {
        switch(weaponData.bulletSpreadType)
        {
            case BulletSpreadType.Shotgun: curSpread *= 5f;
                break;
        }
        switch(weaponData.fireTriggerType)
        {
            case FireTriggerType.Rapid: curSpread *= 1.4f;
                break;
        }
        return curSpread;
    }

    private float CheckFireTimer(WeaponData weaponData, float curFireTimer)
    {
        switch (weaponData.bulletSpreadType)
        {
            case BulletSpreadType.Shotgun:
                curFireTimer *= 5f;
                break;
        }
        switch (weaponData.fireTriggerType)
        {
            case FireTriggerType.Rapid:
                curFireTimer *= 0.75f;
                break;
        }
        return curFireTimer;
    }

    private GameObject CheckBulletType(WeaponData weaponData)
    {
        GameObject bullet = null;
        switch (weaponData.bulletType)
        {
            case BulletType.Normal:
                bullet = PoolingManager.Instance.LentalObj(weaponData.bulletPrefabName);
                Debug.Log(bullet.name);
                break;
            case BulletType.Laser:
                bullet = Instantiate(testLaserBulletPrefab);
                break;
        }
        return bullet;
    }

    private float suckingItemTime = 3f;
    private float suckingItemTimer;
    [SerializeField] private ItemPicker curItemPicker;
    [SerializeField] private List<ItemPicker> itemPickerList = new List<ItemPicker>();

    private float rechargeTimer;
    public void CheckSuck()
    {
        if (curAttackMode == AttackMode.Suck)
        {
            rechargeTimer = 0.2f;
            Sucktion();
        }
        else
        {
            rechargeTimer -= Time.deltaTime;
            rechargeTimer = Mathf.Clamp(rechargeTimer, 0f, 5f);
            if(rechargeTimer <= 0f) Recharge();
        }
    }

    public void Sucktion()
    {
        if (suctionStat.curSuctionRatio.Value <= 0f)
        {
            fovSprite.color = this.suctionStat.fovIdleColor;
            suckingItemTimer = 0f;
            return;
        }

        float amount = (1f / suctionStat.maxSuctionTime) * Time.deltaTime;

        if (suctionStat.curSuctionRatio.Value < amount)
        {
            fovSprite.color = suctionStat.fovIdleColor;
            return;
        }

        itemPickerList.Clear();

        suctionStat.curSuctionRatio.Value = Mathf.Clamp(suctionStat.curSuctionRatio.Value - amount, 0f, 1f);

        fovSprite.color = suctionStat.fovSuctionColor;

        var cols = Physics2D.OverlapCircleAll(this.transform.position, suctionStat.suctionRange, suctionStat.targetLayer);

        foreach (var col in cols)
        {
            Vector3 targetPos = col.transform.position;
            Vector2 targetDir = (targetPos - this.transform.position).normalized;

            var tempLookDir = Funcs.DegreeAngle2Dir(-this.transform.eulerAngles.z);
            //lookDir�� ���ٸ��� �̰ŷ� ����� �ϴ� ���߿� ��
            float angleToTarget = Mathf.Acos(Vector2.Dot(targetDir, tempLookDir)) * Mathf.Rad2Deg;

            //�������ְ� ���� ���� ������ ���ڻ��ΰɾ��ְ� ���Ϸ������� ��ȯ.
            if (angleToTarget <= (suctionStat.suctionAngle))
            {
                //���⼭ �Ѿ˵� ���� ��� ��
                Suckable suckableObj = col.gameObject.GetComponent<Suckable>();
                if (suckableObj)
                {
                    suckableObj.transform.SetParent(null);
                    suckableObj.Sucked(this.transform);
                    owner.inventroy.bulletCount.Value++;
                }

                Holdable holdableObj = col.gameObject.GetComponent<Holdable>();
                if(holdableObj)
                {
                    holdableItem = holdableObj;
                    holdableObj.Hold(firePos);
                    owner.attackController.isSuckPossible = false;
                }
            }
        }
    }

    public void Recharge()
    {
        fovSprite.color = this.suctionStat.fovIdleColor;
        suctionStat.curSuctionRatio.Value = Mathf.Clamp(suctionStat.curSuctionRatio.Value + (1 / suctionStat.rechargeTime * Time.deltaTime), 0f, 1f);
    }

    public void ChangeImageSortOrder(SpriteRenderer sr, string layerName, int order)
    {
        sr.sortingLayerName = layerName;
        sr.sortingOrder = order;
    }

    private void PlayWeaponRecoilEffect()
    {
        //weaponSpriteRenderer.do
    }
}
