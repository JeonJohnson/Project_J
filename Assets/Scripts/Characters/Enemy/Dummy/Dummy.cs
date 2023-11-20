using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    public Rigidbody2D Rigidbody2D { get; private set; }
    public int hp = 10;

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override void Hit(int dmg, Vector2 dir)
    {
        base.Hit(dmg, dir);
        hp -= dmg;
        Rigidbody2D.AddForce(dir * 10);
    }
}
