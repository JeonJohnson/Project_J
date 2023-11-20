using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [System.Serializable]
    public struct Stat
    {
        public float fireRate;
        public Color fovIdleColor;
        public Color fovSuctionColor;
        public float curSuctionRatio;
        public float maxSuctionTime;
        public float rechargeTime;
        public float suctionAngle;
        public float suctionRange;
    }

    public Stat stat;
    private float fireTimer;
    private Player owner;

    public SpriteRenderer weaponSprite;
    public SpriteRenderer fovSprite;
    public Transform jarMouthTr;

    [Header("TEST FUNCS")]
    public GameObject testBulletPrefab;

    private void Update()
    {

    }

    public void Init(Player player)
    {
        owner = player;
        fireTimer = stat.fireRate;
        fovSprite.material.SetFloat("_FovAngle", stat.suctionAngle);
        fovSprite.transform.localScale = new Vector2(stat.suctionRange * 2, stat.suctionRange * 2);
    }

    public void Fire()
    {
        fireTimer -= Time.deltaTime;
        fireTimer = Mathf.Clamp(fireTimer, 0, stat.fireRate);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if(fireTimer <= 0)
            {
                //탕
                GameObject bullet = Instantiate(testBulletPrefab);
                bullet.transform.position = jarMouthTr.position;
                bullet.GetComponent<Bullet>().Fire(jarMouthTr.up, 5);
                fireTimer = stat.fireRate;
            }
        }
    }

    public void Suction()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (stat.curSuctionRatio <= 0f)
            {
                //Recharging();
                fovSprite.color = this.stat.fovIdleColor;
                return;
            }

            float amount = (1f / stat.maxSuctionTime) * Time.deltaTime;

            if (stat.curSuctionRatio < amount)
            {
                //Recharging();
                fovSprite.color = stat.fovIdleColor;
                return;
            }

            stat.curSuctionRatio = Mathf.Clamp(stat.curSuctionRatio - amount, 0f, 1f);

            fovSprite.color = stat.fovSuctionColor;

            var cols = Physics2D.OverlapCircleAll(this.transform.position, stat.suctionRange, LayerMask.GetMask("Bullet"));

            foreach (var col in cols)
            {
                Vector3 targetPos = col.transform.position;
                Vector2 targetDir = (targetPos - this.transform.position).normalized;

                var tempLookDir = Funcs.DegreeAngle2Dir(-this.transform.eulerAngles.z);
                //lookDir랑 값다른데 이거로 적용됨 일단 나중에 ㄱ
                float angleToTarget = Mathf.Acos(Vector2.Dot(targetDir, tempLookDir)) * Mathf.Rad2Deg;

                //내적해주고 나온 라디안 각도를 역코사인걸어주고 오일러각도로 변환.
                if (angleToTarget <= (stat.suctionAngle * 0.5f))
                {
                    //여기서 총알들 한테 흡수 ㄱ
                    Debug.Log(col.gameObject.name);
                    Bullet bullet = col.gameObject.GetComponent<Bullet>();
                    Debug.Log(bullet);
                    if (bullet)
                    {
                        bullet.transform.SetParent(null);
                        if (bullet.suckedOption == SuckedOption.Sucked && bullet.curState == BulletState.Fire)
                        {
                            bullet.Sucked(owner);
                        }
                    }
                }
            }
        }
        else
        {
            fovSprite.color = this.stat.fovIdleColor;
            Recharge();
        }
    }

    public void Recharge()
    {
        stat.curSuctionRatio = Mathf.Clamp(stat.curSuctionRatio + (1 / stat.rechargeTime * Time.deltaTime), 0f, 1f);
    }
}
