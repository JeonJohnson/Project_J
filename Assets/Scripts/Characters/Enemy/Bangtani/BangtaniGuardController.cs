using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BangtaniGuardController : MonoBehaviour
{
    private Bangtani bangtani;

    public bool isGuarding = false;
    public Transform shieldTr;
    public SpriteRenderer shieldSprite;

    private void Awake()
    {
        bangtani = GetComponent<Bangtani>();
    }

    private void Update()
    {
        if(isGuarding)
        {
            shieldSprite.enabled = true;
            Vector3 dir = bangtani.target.transform.position - shieldTr.position;
            dir.Normalize();
            shieldTr.transform.rotation = Quaternion.LookRotation(shieldTr.forward, dir);
        }
        else
        {
            shieldSprite.enabled = false;
        }
    }

    public bool GetIsGuardSucess(Vector3 bulletDir, float guardableAngle)
    {
        float angleDiff = Vector3.Angle(bulletDir, shieldTr.transform.up);

        if (angleDiff <= guardableAngle)
        {
            bangtani.status.isDurable = true;
            return true;
        }
        else
        {
            bangtani.status.isDurable = false;
            return false;
        }
    }
}
