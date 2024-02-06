using JetBrains.Annotations;
using System;

public enum BulletType
{
    Non, Normal, Laser
}

public enum BulletEffect
{
    Non, Normal, Trace
}

public enum BulletSpreadType
{
    Non, Normal, Shotgun
}

public enum FireTriggerType
{
    Non, Normal, Rapid, Charge
}

[Serializable]
public struct WeaponData
{
    //weapon
    public float fireRate;
    public int bulletNumPerFire;
    public int decreaseBulletNumPerFire;
    public float spread;

    //bullet
    public float bulletSpeed;
    public float bulletSize;
    public int damage;
    public float critical;
    public int magSize;

    public BulletType bulletType;
    public BulletEffect bulletEffect;
    public BulletSpreadType bulletSpreadType; 
    public FireTriggerType fireTriggerType;

    public string bulletPrefabName;
}