using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Player : Weapon
{
    [System.Serializable]
    public struct SuctionStat
    {
        public Color fovIdleColor;
        public Color fovSuctionColor;
        public float curSuctionRatio;
        public float maxSuctionTime;
        public float rechargeTime;
        public float suctionAngle;
        public float suctionRange;

        public LayerMask targetLayer;
    }

    private Player owner;
    public SuctionStat suctionStat;
    public SpriteRenderer fovSprite;

    private void Update()
    {

    }

    public override void Init(CObj _owner)
    {
        base.Init(_owner);
        owner = (Player) _owner;
        fireTimer = stat.fireRate;
        fovSprite.material.SetFloat("_FovAngle", suctionStat.suctionAngle);
        fovSprite.transform.localScale = new Vector2(suctionStat.suctionRange * 2, suctionStat.suctionRange * 2);
    }

    public override void Fire()
    {
        fireTimer -= Time.deltaTime;
        fireTimer = Mathf.Clamp(fireTimer, 0, stat.fireRate);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (fireTimer <= 0)
            {
                //��
                GameObject bullet = Instantiate(testBulletPrefab);
                bullet.transform.position = firePos.position;
                bullet.GetComponent<Bullet>().Fire(firePos.up, 5);
                fireTimer = stat.fireRate;
            }
        }
    }

    public void Suction()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (suctionStat.curSuctionRatio <= 0f)
            {
                //Recharging();
                fovSprite.color = this.suctionStat.fovIdleColor;
                return;
            }

            float amount = (1f / suctionStat.maxSuctionTime) * Time.deltaTime;

            if (suctionStat.curSuctionRatio < amount)
            {
                //Recharging();
                fovSprite.color = suctionStat.fovIdleColor;
                return;
            }

            suctionStat.curSuctionRatio = Mathf.Clamp(suctionStat.curSuctionRatio - amount, 0f, 1f);

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
                if (angleToTarget <= (suctionStat.suctionAngle * 0.5f))
                {
                    //���⼭ �Ѿ˵� ���� ��� ��
                    Debug.Log(col.gameObject.name);
                    Bullet bullet = col.gameObject.GetComponent<Bullet>();
                    Debug.Log(bullet);
                    if (bullet)
                    {
                        bullet.transform.SetParent(null);
                        if (bullet.suckedOption == SuckedOption.Sucked && bullet.curState == BulletState.Fire)
                        {
                            bullet.Sucked((Player)owner);
                        }
                    }
                }
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
        suctionStat.curSuctionRatio = Mathf.Clamp(suctionStat.curSuctionRatio + (1 / suctionStat.rechargeTime * Time.deltaTime), 0f, 1f);
    }
}
