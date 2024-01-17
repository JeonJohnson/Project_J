using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private Player player;
    public Weapon_Player weapon;
    public bool isFirePossible;
    public bool isSuckPossible;

    private bool isFiring = false;
    private bool isGuarding = false;

    private AttackMode attackMode;

    private void Awake()
    {
        player = GetComponent<Player>();
        weapon = player.curWeapon;
        weapon.Init(player);
        isFirePossible = true;
        isSuckPossible = true;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
        if(isFirePossible) weapon.CheckFire();
        if(isSuckPossible) weapon.CheckSuck();
    }

    private void Aim()
    {
        Vector2 MousePosition = Input.mousePosition;
        Vector2 aimPos = Camera.main.ScreenToWorldPoint(MousePosition);

        Vector2 aimDir = (aimPos - new Vector2(player.weaponHolder.position.x, player.weaponHolder.position.y));
        player.weaponHolder.up = aimDir.normalized;
    }
}
