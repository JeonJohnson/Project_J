using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Structs;
using Unity.VisualScripting;

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
    }

    public override void Init(CObj _owner)
    {
        base.Init(_owner);
        owner = (Player) _owner;
        suctionStat.curSuctionRatio = new Data<float>();
        fovSprite.material.SetFloat("_ArcAngle", suctionStat.suctionAngle);
        fovSprite.transform.localScale = new Vector2(suctionStat.suctionRange * 2, suctionStat.suctionRange * 2);
    }


    public override void Fire()
    {
        if (owner.inventroy.bulletCount.Value <= 0) return;

        WeaponData weaponData = owner.inventroy.curWeaponSlot.weaponData;

        // 발사방식 체크
        if (!CheckFireType(weaponData.fireTriggerType, KeyCode.Mouse0)) return;

        // 각도 체크
        float spreadAngle = weaponData.spread;
        //spreadAngle = CheckSpreadAngle(weaponData, spreadAngle);

        // 총알갯수 체크
        int bulletNum = weaponData.bulletNumPerFire;

        // 총알속도 체크
        float bulletSpeed = weaponData.bulletSpeed;

        // 총알 크기 체크
        float bulletSize = weaponData.bulletSize;

        // 총알 데미지 체크

        int dmg = Mathf.CeilToInt(weaponData.damage);

        float criticalValue = weaponData.critical;
        criticalValue = Mathf.Clamp(criticalValue, 0, 100);
        int rndValue = Random.Range(0, 100);
        if (rndValue < criticalValue) dmg = dmg * 2;

        // 총알종류 체크 & 발사

        for (int i = 0; i < bulletNum; i++)
        {
            if (owner.inventroy.bulletCount.Value <= 0) return;
            GameObject bullet = CheckBulletType(weaponData);
            bullet.transform.position = firePos.transform.position;
            float rnd = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Quaternion rndRot = Quaternion.Euler(0f, 0f, rnd);
            Vector2 rndDir = rndRot * firePos.up;
            rndDir.Normalize();
            bullet.GetComponent<Bullet>().Fire(rndDir, 5, bulletSpeed, bulletSize);
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
        //        //탕
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

    public void Suction()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (suctionStat.curSuctionRatio.Value <= 0f)
            {
                //Recharging();
                fovSprite.color = this.suctionStat.fovIdleColor;
                suckingItemTimer = 0f;
                return;
            }

            float amount = (1f / suctionStat.maxSuctionTime) * Time.deltaTime;

            if (suctionStat.curSuctionRatio.Value < amount)
            {
                //Recharging();
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
                //lookDir랑 값다른데 이거로 적용됨 일단 나중에 ㄱ
                float angleToTarget = Mathf.Acos(Vector2.Dot(targetDir, tempLookDir)) * Mathf.Rad2Deg;

                //내적해주고 나온 라디안 각도를 역코사인걸어주고 오일러각도로 변환.
                if (angleToTarget <= (suctionStat.suctionAngle))
                {
                    //여기서 총알들 한테 흡수 ㄱ
                    Suckable suckableObj = col.gameObject.GetComponent<Suckable>();
                    if (suckableObj)
                    {
                        suckableObj.transform.SetParent(null);
                        suckableObj.Sucked(this.transform);
                        owner.inventroy.bulletCount.Value++;
                    }
                    else
                    {
                        ItemPicker itemPicker = col.gameObject.GetComponent<ItemPicker>();
                        if (itemPicker)
                        {
                            itemPickerList.Add(itemPicker);
                            //itemPicker.Sucking(owner);
                        }
                    }
                }
            }

            if (itemPickerList.Count > 0)
            {
                if(curItemPicker == null)
                {
                    curItemPicker = itemPickerList[0];
                }
                else if (!itemPickerList.Contains(curItemPicker))
                {
                    curItemPicker = itemPickerList[0];
                }
            }
            else
            {
                curItemPicker = null;
            }

            if (curItemPicker != null)
            {
                //if(!curItemPicker.Sucking(owner)) curItemPicker = null;
            }
        }
        else
        {
            fovSprite.color = this.suctionStat.fovIdleColor;
            Recharge();
        }
    }

    public void Recharge()
    {
        suctionStat.curSuctionRatio.Value = Mathf.Clamp(suctionStat.curSuctionRatio.Value + (1 / suctionStat.rechargeTime * Time.deltaTime), 0f, 1f);
    }
}
