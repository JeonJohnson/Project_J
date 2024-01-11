using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Suckable : MonoBehaviour
{
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
        public MoreMountains.TopDownEngine.Weapon suckedWeapon;
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

    private Color defColor;

    private void Awake()
    {
        defColor = srdr.color;
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Sucked(MoreMountains.TopDownEngine.Weapon _suckedWeapon)
    {
        projectile = this.gameObject.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.DistanceLimit = false;
            projectile.enabled = false;
        }
        if (rb != null) rb.velocity = Vector3.zero;

        srdr.color = srdr.color * 5f;
        curState = BulletState.SuckWait;
        suckedStat.suckWaitRandTime = UnityEngine.Random.Range(0.1f, 0.25f);

        col.enabled = false;
        suckedStat.suckedWeapon = _suckedWeapon;

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
            transform.position = Vector2.Lerp(suckedStat.suckStartPos, suckedStat.suckedWeapon.transform.position, suckedStat.suckingTimeRatio);
            srdr.color = new Color(srdr.color.r, srdr.color.g, srdr.color.b, 1 - suckedStat.suckingTimeRatio * 2f);
            yield return null;
        }

        LevelManager.Instance.Players[0].GetComponent<CharacterInventory>().MainInventory.AddItem(item, 1);
        item.Equip("Player1");

        Resetting();
        if (projectile != null) projectile.enabled = true;
        //�����ϱ�
        this.gameObject.SetActive(false);
    }

    public void Resetting()
    {
        col.enabled = true;
        srdr.color = Color.white;
        curState = BulletState.Fire;
    }
}	
