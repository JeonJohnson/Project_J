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
public struct WeaponUpgradeData
{
    public BulletType bulletType;
    public BulletEffect bulletEffect;
    public BulletSpreadType bulletSpreadType; 
    public FireTriggerType fireTriggerType;
}