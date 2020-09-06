using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct SteeringAgent : IComponentData
{
    public float3 TargetPosition;
}

[Serializable]
public struct Velocity : IComponentData
{
    public float3 Value;
}
