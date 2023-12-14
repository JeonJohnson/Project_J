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

    public WeaponStatus defaltStatus;
    public WeaponUpgradeData upgradeData;



    private void Awake()
    {

    }

    private void Start()
    {
       // upgradeData.bulletType = BulletType.Laser;
        defaltStatus.bulletCount.Value = 50;
    }

    private void Update()
    {

    }

    public override void Init(CObj _owner)
    {
        base.Init(_owner);
        owner = (Player) _owner;
        defaltStatus.bulletCount = new Data<int>();
        suctionStat.curSuctionRatio = new Data<float>();
        fireTimer = defaltStatus.fireRate;
        fovSprite.material.SetFloat("_FovAngle", suctionStat.suctionAngle);
        fovSprite.transform.localScale = new Vector2(suctionStat.suctionRange * 2, suctionStat.suctionRange * 2);
    }

    public override void Fire()
    {
        if (defaltStatus.bulletCount.Value <= 0) return;

        // 발사방식 체크
        if(!CheckFireType(upgradeData.fireTriggerType, KeyCode.Mouse0)) return;

        // 각도 체크
        float spreadAngle = defaltStatus.bulletSpread + owner.inventroy.invenBonusStatus.bonus_Weapon_Spread + owner.bonusStatus.bonus_Weapon_Spread;
        spreadAngle = CheckSpreadAngle(upgradeData, spreadAngle);

        // 총알갯수 체크
        int bulletNum = defaltStatus.bulletNumPerFire + owner.inventroy.invenBonusStatus.bonus_Weapon_BulletNumPerFire + owner.bonusStatus.bonus_Weapon_BulletNumPerFire;

        // 총알속도 체크
        float bulletSpeed = defaltStatus.bulletSpeed + owner.inventroy.invenBonusStatus.bonus_Weapon_Speed + owner.bonusStatus.bonus_Weapon_Speed;

        // 총알 크기 체크
        float bulletSize = defaltStatus.bulletSize + owner.inventroy.invenBonusStatus.bonus_Weapon_BulletSize + owner.bonusStatus.bonus_Weapon_BulletSize;

        // 총알 데미지 체크

        int dmg = Mathf.CeilToInt(defaltStatus.damage + owner.inventroy.invenBonusStatus.bonus_Weapon_Damage + owner.bonusStatus.bonus_Weapon_Damage);

        float criticalValue = defaltStatus.critial + owner.inventroy.invenBonusStatus.bonus_Weapon_Critial + owner.bonusStatus.bonus_Weapon_Critial;
        criticalValue = Mathf.Clamp(criticalValue, 0, 100);
        int rndValue = Random.Range(0, 100);
        if (rndValue < criticalValue) dmg = dmg * 2;

        // 총알종류 체크 & 발사

        for (int i = 0; i < bulletNum; i++)
        {
            if (defaltStatus.bulletCount.Value <= 0) return;
            GameObject bullet = CheckBulletType(upgradeData);
            bullet.transform.position = firePos.transform.position;
            float rnd = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Quaternion rndRot = Quaternion.Euler(0f, 0f, rnd);
            Vector2 rndDir = rndRot * firePos.up;
            rndDir.Normalize();
            bullet.GetComponent<Bullet>().Fire(rndDir, 5, bulletSpeed, bulletSize);
            defaltStatus.bulletCount.Value--;
        }
        fireTimer = defaltStatus.fireRate * (1f - owner.inventroy.invenBonusStatus.bonus_Weapon_FireRate / 100f);
        fireTimer = CheckFireTimer(upgradeData, fireTimer);
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
        switch (triggerType) 
        {
            case FireTriggerType.Normal:
                fireTimer -= Time.deltaTime;
                fireTimer = Mathf.Clamp(fireTimer, 0, defaltStatus.fireRate);
                if (Input.GetKeyDown(KeyCode.Mouse0) && fireTimer <= 0) return true; else return false;
            case FireTriggerType.Rapid:
                fireTimer -= Time.deltaTime;
                fireTimer = Mathf.Clamp(fireTimer, 0, defaltStatus.fireRate);
                if (Input.GetKey(KeyCode.Mouse0) && fireTimer <= 0) return true; else return false;
            case FireTriggerType.Charge:
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    fireTimer -= Time.deltaTime;
                    fireTimer = Mathf.Clamp(fireTimer, 0, defaltStatus.fireRate);
                    if (fireTimer <= 0) return true;
                    else return false;
                }
                else 
                { 
                    fireTimer = defaltStatus.fireRate;
                    return false;
                }
        }
        return false;
    }

    private float CheckSpreadAngle(WeaponUpgradeData weaponUpgradeData, float curSpread)
    {
        switch(weaponUpgradeData.bulletSpreadType)
        {
            case BulletSpreadType.Shotgun: curSpread *= 5f;
                break;
        }
        switch(weaponUpgradeData.fireTriggerType)
        {
            case FireTriggerType.Rapid: curSpread *= 1.4f;
                break;
        }
        return curSpread;
    }

    private float CheckFireTimer(WeaponUpgradeData weaponUpgradeData, float curFireTimer)
    {
        switch (weaponUpgradeData.bulletSpreadType)
        {
            case BulletSpreadType.Shotgun:
                curFireTimer *= 5f;
                break;
        }
        switch (weaponUpgradeData.fireTriggerType)
        {
            case FireTriggerType.Rapid:
                curFireTimer *= 0.75f;
                break;
        }
        return curFireTimer;
    }

    private GameObject CheckBulletType(WeaponUpgradeData weaponUpgradeData)
    {
        GameObject bullet = null;
        switch (weaponUpgradeData.bulletType)
        {
            case BulletType.Normal:
                bullet = Instantiate(testBulletPrefab);
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
                    Bullet bullet = col.gameObject.GetComponent<Bullet>();
                    if (bullet)
                    {
                        bullet.transform.SetParent(null);
                        if (bullet.suckedOption == SuckedOption.Sucked && bullet.curState == BulletState.Fire)
                        {
                            bullet.Sucked((Player)owner);
                            defaltStatus.bulletCount.Value++;
                        }
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
