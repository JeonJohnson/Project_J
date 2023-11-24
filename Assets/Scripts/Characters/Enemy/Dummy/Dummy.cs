using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    public Rigidbody2D Rigidbody2D { get; private set; }
    public int hp = 10;

    public override void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override HitInfo Hit(int dmg, Vector2 dir)
    {
        base.Hit(dmg, dir);
        hp -= dmg;
        Rigidbody2D.AddForce(dir * 10);

        HitInfo hitInfo = new HitInfo();
        return hitInfo;
    }

    protected override void Initialize()
    {
        throw new System.NotImplementedException();
    }
}
