using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestDemoFireTimerGauge : MonoBehaviour
{
    [SerializeField] Image gaugeImg;
    private void Update()
    {
        gaugeImg.fillAmount = IngameController.Instance.Player.curWeapon.isSuckDemo_AttackTimer / 14f;
    }
}
