using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private Player player;
    private Weapon_Player weapon;
    public bool isFirePossible;

    private void Awake()
    {
        player = GetComponent<Player>();
        weapon = player.curWeapon;
    }

    void Start()
    {
        weapon.Init(player);
        isFirePossible = true;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
        if (isFirePossible)
        {
            weapon.Fire();
            weapon.Suction();
        }
    }

    private void Aim()
    {
        Vector2 MousePosition = Input.mousePosition;
        Vector2 aimPos = Camera.main.ScreenToWorldPoint(MousePosition);

        Vector2 aimDir = (aimPos - new Vector2(player.weaponHolder.position.x, player.weaponHolder.position.y));
        player.weaponHolder.up = aimDir.normalized;
    }
}
