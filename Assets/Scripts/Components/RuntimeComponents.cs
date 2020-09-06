using System;
using Unity.Entities;

[Serializable]
public struct Projectile : IComponentData { }

public struct Gravity : IComponentData { }

[Serializable]
public struct Health : IComponentData
{
    public int Value;
}

[Serializable]
public struct TimeToLive : IComponentData
{
    public float Value;
}
